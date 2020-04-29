/*
 * Lab_9_ATmega2560.c
 *
 * Created: 17.04.2020 18:22:15
 * Author : Serhii-PC
 */ 

/*
 * variant 12
 
 * 
 * 
 */ 

//#define F_CPU 16000000UL
#define F_CPU 1843200UL

#include <avr/io.h>
#include <util/delay.h>
#include <avr/interrupt.h>

#define BAUD_RS_485 19200
#define BAUD_RS_232 9600
#define UBRR_CALC_RS_485 (F_CPU/(BAUD_RS_485*16L) - 1)
#define UBRR_CALC_RS_232 (F_CPU/(BAUD_RS_232*16L) - 1)
#define BUF_SIZE_RX 128
#define BUF_SIZE_TX 128//check//max 256
#define BUF_MASK_RX (BUF_SIZE_RX - 1)
#define BUF_MASK_TX (BUF_SIZE_TX - 1)

#define ADDRESS_SLAVE_1 '1'
#define ADDRESS_SLAVE_2 '2'
#define COMMAND_WRITE 'w'//0xA1
#define COMMAND_READ  'r'//0xB1
#define EN_m PD1
volatile int8_t bufRXfromPC[BUF_SIZE_RX];
volatile int8_t bufTXfromPC[BUF_SIZE_TX];

volatile int8_t bufRXfromSlave[BUF_SIZE_RX];
volatile int8_t bufTXfromSlave[BUF_SIZE_RX];

volatile uint8_t rxInPC = 0, rxOutPC = 0, rxInSlave = 0, rxOutSlave = 0;
volatile uint8_t txInPC = 0, txOutPC = 0, txInSlave = 0, txOutSlave = 0;

uint8_t flag = 1;//0 - bit isAddress, 1 bit - isCommand

uint8_t readBufRXfromPC(void);
void writeBufTXfromPC(uint8_t value);
uint8_t readBufRXfromSlave(void);
void writeBufTXfromSlave(uint8_t value);
void setWriteModeRS485(void);
void print(char *str);
void Setup(void)
{
	//En_m 
	DDRD |= (1 << EN_m);
	PORTD &= ~(1 << EN_m);
	//setup USART0 (master - PC) (RS-232)
	
	UCSR0A = 0;
	//interruption of receive, work receiver, transmitter:
	UCSR0B = (1 << RXCIE0)|(1 << UDRIE0)|(1 << RXEN0)|(1 << TXEN0);
	//8 bit, no parity, 1 stop bit:
	UCSR0C = (1 << UCSZ01)|(1 << UCSZ00);
	//speed : 9600 bps:
	UBRR0H = (uint8_t)(UBRR_CALC_RS_232 >> 8);
	UBRR0L = (uint8_t)(UBRR_CALC_RS_232);
	 
	//setup USART1 (master - slave1, slave 2) (RS-485)
	
	UCSR1A = 0;
	//interruption of receive and transmit,UDRIE1, work receiver, transmitter:
	UCSR1B = (1 << RXCIE1)|(1 << TXCIE1)|(1 << UDRIE1)|(1 << RXEN1)|(1 << TXEN1)|(1 << UCSZ12);
	//9 bit, no parity, 1 stop bit:
	UCSR1C = (1 << UCSZ11)|(1 << UCSZ10);//(1 << UCSZ12);
	//speed : 19200 bps:
	UBRR1H = (uint8_t)(UBRR_CALC_RS_485 >> 8);
	UBRR1L = (uint8_t)(UBRR_CALC_RS_485);
}
/*************Master-PC*******************************/
//receiver from PC
ISR(USART0_RX_vect)
{
	bufRXfromPC[rxInPC++] = UDR0;
	rxInPC &= BUF_MASK_RX;
    //added overflow check
}

////transmit from PC
ISR(USART0_UDRE_vect)
{
	UDR0 = bufTXfromPC[txOutPC++];
	txOutPC &= BUF_MASK_TX;
	if(txOutPC == txInPC)
		UCSR0B &= ~(1 << UDRIE0);
}
/*****************Master-Slaves**********************************/

//receiver from slaves
ISR(USART1_RX_vect)
{
	bufRXfromSlave[rxInSlave++] = UDR1;
	rxInSlave &= BUF_MASK_RX;
}

ISR(USART1_TX_vect)
{
	PORTD &= ~(1 << EN_m);
}

ISR(USART1_UDRE_vect)
{
	UDR1 = bufTXfromSlave[txOutSlave++];
	txOutSlave &= BUF_MASK_TX;
	if(txOutSlave == txInSlave)
		UCSR1B &= ~(1 << UDRIE1);
}

int main(void)
{
    Setup();
	_delay_ms(200);
	sei();
	uint8_t byteFromPC = 0;
	uint8_t byteFromSlave = 0;
	uint8_t command = 0;
	flag = 1;
    while (1) 
    {
		if(rxOutPC != rxInPC)//if available
		{
			byteFromPC = readBufRXfromPC();//check
			if(flag == 1){
				setWriteModeRS485();
				UCSR1B |= (1 << TXB81);
				writeBufTXfromSlave(byteFromPC);
				flag = (1 << 1);
			}
			else if(flag == (1 << 1)){
				flag = 0;
				command = byteFromPC;
				setWriteModeRS485();
				UCSR1B &= ~(1 << TXB81);
				writeBufTXfromSlave(byteFromPC);
				if(command == COMMAND_READ){
					flag = 1;
				}
			}
			else{
				flag = 1;//set isAddress
				setWriteModeRS485();
				UCSR1B &= ~(1 << TXB81);
				writeBufTXfromSlave(byteFromPC);
			}
			//setWriteModeRS485();
			//writeBufTXfromSlave(byteFromPC);
		}
		if(rxOutSlave != rxInSlave)
		{
			writeBufTXfromPC(readBufRXfromSlave());
		}
		
		//print("QWERTY");
		//_delay_ms(10);
		//for(int i = 0;i < 255;++i){
			//_delay_ms(10);
			//writeBufTXfromPC(i);
		//}
	}
	return 0;
}

uint8_t readBufRXfromPC(void)
{
	uint8_t value = bufRXfromPC[rxOutPC++];
	rxOutPC &= BUF_MASK_RX;
	return value;
}

void writeBufTXfromPC(uint8_t value)
{
	bufTXfromPC[txInPC++] = value;
	txInPC &= BUF_MASK_TX;
	//added overflow check
	cli();
	UCSR0B |= (1 << UDRIE0);
	sei();
}


uint8_t readBufRXfromSlave(void)
{
	uint8_t value = bufRXfromSlave[rxOutSlave++];
	rxOutSlave &= BUF_MASK_RX;
	return value;
}

void writeBufTXfromSlave(uint8_t value)
{
	bufTXfromSlave[txInSlave++] = value;
	txInSlave &= BUF_MASK_TX;
	//added overflow check
	UCSR1B |= (1 << UDRIE1);
}

void setWriteModeRS485(void)
{
	PORTD |= (1 << EN_m);//enable transmit from slave mode
	_delay_ms(1);
}

void print(char *str)
{
	int i = 0;
	while(str[i] != '\0')
	{
		//_delay_ms(1);
		writeBufTXfromPC(str[i]);
		++i;	
	}
}