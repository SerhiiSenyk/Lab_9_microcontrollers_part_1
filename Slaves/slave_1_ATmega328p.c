/*
 * Lab_9_ATmega328P.c
 *
 * Created: 28.04.2020 15:37:18
 * Author : Serhii-PC
 */ 

//#define F_CPU 16000000UL
#define F_CPU 1843200UL
#include <avr/io.h>
#include <util/delay.h>
#include <avr/interrupt.h>

#define BAUD_RS_485 19200
#define UBRR_CALC_RS_485 (F_CPU/(BAUD_RS_485*16L) - 1)
#define BUF_SIZE_RX 64
#define BUF_SIZE_TX 64
#define BUF_MASK_RX (BUF_SIZE_RX - 1)
#define BUF_MASK_TX (BUF_SIZE_TX - 1)

#define ADDRESS '2'
#define COMMAND_WRITE 'w'//0xA1
#define COMMAND_READ  'r'//0xB1
#define ENABLE PORTD2
volatile int8_t bufRX[BUF_SIZE_RX];
volatile int8_t bufTX[BUF_SIZE_RX];
volatile uint8_t rxIn = 0, rxOut = 0, txIn = 0, txOut = 0;
uint8_t flag = 1;//0 bit - isAddress, 1 bit - isCommand, flag = 0 - isData
uint8_t readBufRX(void);
void writeBufTX(uint8_t value);
void print(char *str);
void setWriteModeRS485(void);
void Setup()
{
	UCSR0A |= (1 << MPCM0);
	//interruption of receive and transmit, work receiver, transmitter:
	UCSR0B = (1 << RXCIE0)|(1 << TXCIE0)|(1 << UDRIE0)|(1 << RXEN0)|(1 << TXEN0)|(1 << UCSZ02);//(1 << TXCIE0)
	//8 bit, no parity, 1 stop bit:
	UCSR0C = (1 << UCSZ01)|(1 << UCSZ00);//(1 << UCSZ02)
	//speed : 19200 bps:
	UBRR0H = (uint8_t)(UBRR_CALC_RS_485 >> 8);
	UBRR0L = (uint8_t)(UBRR_CALC_RS_485);
	
	
	DDRD |= (1 << ENABLE);
	PORTD |= (1 << ENABLE);
}

ISR(USART_RX_vect)
{
	bufRX[rxIn++] = UDR0;
	rxIn &= BUF_MASK_RX;
	//added overflow check
}

ISR(USART_UDRE_vect)
{
	UDR0 = bufTX[txOut++];
	txOut &= BUF_MASK_TX;
	if(txOut == txIn)
		UCSR0B &= ~(1 << UDRIE0);
}

ISR(USART_TX_vect)
{
	PORTD &= ~(1 << ENABLE);
}

int main(void)
{
    Setup();
	_delay_ms(500);
	sei();
	uint8_t byteFromMaster = 0;
	flag = 1;
	uint8_t data = 'L';
	uint8_t command = 0;
    while (1) //36:14
    {
		if(rxOut != rxIn)
		{
			byteFromMaster = readBufRX();
			if(flag == 1){
				if(byteFromMaster == ADDRESS){
					flag = (1 << 1);
					UCSR0A &= ~(1 << MPCM0);
				}
			}
			else if(flag == (1 << 1)){
				flag = 0;
				command = byteFromMaster;
				if(command == COMMAND_READ){
					flag = 1;
					setWriteModeRS485();
					writeBufTX(data);
					UCSR0A |= (1 << MPCM0);//check
				}
			}
			else{
				flag = 1;
				if(command == COMMAND_WRITE){
					flag = 1;
					//PORTB = byteFromMaster;	
					setWriteModeRS485();
					writeBufTX(byteFromMaster);
				}
				UCSR0A |= (1 << MPCM0);//check
			}
		}
	}
	return 0;
}

uint8_t readBufRX(void)
{
	uint8_t value = bufRX[rxOut++];
	rxOut &= BUF_MASK_RX;
	return value;	
}

void writeBufTX(uint8_t value)
{
	bufTX[txIn++] = value;
	txIn &= BUF_MASK_TX;
	//added overflow check
	UCSR0B |= (1 << UDRIE0);
}

void setWriteModeRS485(void)
{
	PORTD |= (1 << ENABLE);//transmit mode
	_delay_ms(1);
}

void print(char *str)
{
	int i = 0;
	while(str[i] != '\0')
	{
		//_delay_ms(1);
		writeBufTX(str[i]);
		++i;
	}
}
