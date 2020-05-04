/*
 * Lab_9_ATmega328p_slave2.c
 *
 * Created: 29.04.2020 23:35:17
 * Author : Serhii-PC
 */ 

//#define F_CPU 16000000UL
#define F_CPU 1843200UL///////
//#define F_CPU 7372800UL
#include <avr/io.h>
#include <util/delay.h>
#include <avr/interrupt.h>
#include <avr/pgmspace.h>
#define BAUD_RS_485 19200
#define UBRR_CALC_RS_485 (F_CPU/(BAUD_RS_485*16L) - 1)
#define BUF_SIZE_RX 64
#define BUF_SIZE_TX 64
#define BUF_MASK_RX (BUF_SIZE_RX - 1)
#define BUF_MASK_TX (BUF_SIZE_TX - 1)

#define ADDRESS 0x45
#define COMMAND_WRITE 0xA1
#define COMMAND_READ  0xB1
#define ENABLE PORTD2
#define SIZE_CRC_BLOCK 4
volatile int8_t bufRX[BUF_SIZE_RX];
volatile int8_t bufTX[BUF_SIZE_RX];
volatile uint8_t rxIn = 0, rxOut = 0, txIn = 0, txOut = 0;
volatile uint8_t flag = 1;//0 bit - isAddress, 1 bit - isCommand, flag = 0 - isData
uint8_t distortion = 0;
uint8_t readBufRX(void);
void writeBufTX(uint8_t value);
void print(char *str);
void setWriteModeRS485(void);
void print_with_crc(const char *str);
uint8_t calc_crc8_rohs(uint8_t *block, uint8_t size);
const uint8_t CRC_8_ROHS_table[0x100] PROGMEM = {
	0x0, 0x91, 0xe3, 0x72, 0x7, 0x96, 0xe4, 0x75, 0xe, 0x9f, 0xed, 0x7c, 0x9,
	0x98, 0xea, 0x7b, 0x1c, 0x8d, 0xff, 0x6e, 0x1b, 0x8a, 0xf8, 0x69, 0x12, 0x83, 0xf1, 0x60, 0x15,
	0x84, 0xf6, 0x67, 0x38, 0xa9, 0xdb, 0x4a, 0x3f, 0xae, 0xdc, 0x4d, 0x36, 0xa7, 0xd5, 0x44, 0x31,
	0xa0, 0xd2, 0x43, 0x24, 0xb5, 0xc7, 0x56, 0x23, 0xb2, 0xc0, 0x51, 0x2a, 0xbb, 0xc9, 0x58, 0x2d,
	0xbc, 0xce, 0x5f, 0x70, 0xe1, 0x93, 0x2, 0x77, 0xe6, 0x94, 0x5, 0x7e, 0xef, 0x9d, 0xc, 0x79,
	0xe8, 0x9a, 0xb, 0x6c, 0xfd, 0x8f, 0x1e, 0x6b, 0xfa, 0x88, 0x19, 0x62, 0xf3, 0x81, 0x10, 0x65,
	0xf4, 0x86, 0x17, 0x48, 0xd9, 0xab, 0x3a, 0x4f, 0xde, 0xac, 0x3d, 0x46, 0xd7, 0xa5, 0x34, 0x41,
	0xd0, 0xa2, 0x33, 0x54, 0xc5, 0xb7, 0x26, 0x53, 0xc2, 0xb0, 0x21, 0x5a, 0xcb, 0xb9, 0x28, 0x5d,
	0xcc, 0xbe, 0x2f, 0xe0, 0x71, 0x3, 0x92, 0xe7, 0x76, 0x4, 0x95, 0xee, 0x7f, 0xd, 0x9c, 0xe9, 0x78,
	0xa, 0x9b, 0xfc, 0x6d, 0x1f, 0x8e, 0xfb, 0x6a, 0x18, 0x89, 0xf2, 0x63, 0x11, 0x80, 0xf5, 0x64,
	0x16, 0x87, 0xd8, 0x49, 0x3b, 0xaa, 0xdf, 0x4e, 0x3c, 0xad, 0xd6, 0x47, 0x35, 0xa4, 0xd1, 0x40,
	0x32, 0xa3, 0xc4, 0x55, 0x27, 0xb6, 0xc3, 0x52, 0x20, 0xb1, 0xca, 0x5b, 0x29, 0xb8, 0xcd, 0x5c,
	0x2e, 0xbf, 0x90, 0x1, 0x73, 0xe2, 0x97, 0x6, 0x74, 0xe5, 0x9e, 0xf, 0x7d, 0xec, 0x99, 0x8, 0x7a,
	0xeb, 0x8c, 0x1d, 0x6f, 0xfe, 0x8b, 0x1a, 0x68, 0xf9, 0x82, 0x13, 0x61, 0xf0, 0x85, 0x14, 0x66,
	0xf7, 0xa8, 0x39, 0x4b, 0xda, 0xaf, 0x3e, 0x4c, 0xdd, 0xa6, 0x37, 0x45, 0xd4, 0xa1, 0x30, 0x42,
	0xd3, 0xb4, 0x25, 0x57, 0xc6, 0xb3, 0x22, 0x50, 0xc1, 0xba, 0x2b, 0x59, 0xc8, 0xbd, 0x2c, 0x5e, 0xcf
};


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
	uint8_t command = 0;
	const char string[30] = "18.09.1998";
	while (1)
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
					print_with_crc(string);
					++distortion;
					if(distortion > 2)
					distortion = 0;
					UCSR0A |= (1 << MPCM0);
				}
			}
			else{
				flag = 1;
				if(command == COMMAND_WRITE){
					flag = 1;
					//PORTB = byteFromMaster;
					//setWriteModeRS485();
					//writeBufTX(byteFromMaster);
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

void print_with_crc(const char *str)//max length 255
{
	uint8_t countDataBytes = 0;
	uint8_t countBlocks = 0;
	uint8_t crc = 0;
	uint8_t bit = 0;
	uint8_t block[SIZE_CRC_BLOCK];
	while(str[countDataBytes] != '\0')
	{
		++countBlocks;
		for (int k = 0; k < SIZE_CRC_BLOCK; ++k)
		{
			if (str[countDataBytes] != '\0')
			{
				block[k] = str[countDataBytes];
				++countDataBytes;
			}
			else {
				block[k] = 0;
			}
		}
		crc = calc_crc8_rohs(block, SIZE_CRC_BLOCK);
		for (int k = 0; k < SIZE_CRC_BLOCK; ++k)
		{
			_delay_ms(1);
			//------------------------------------------
			if(distortion == 1 && countBlocks == 1 && k == 0)//first bytes , 0 bit
			{
				bit = 0;
				if ((block[k] >> bit)& 1)
					block[k] &= ~(1 << bit);
				else
					block[k] |= (1 << bit);
			}
			if(distortion == 2 && countBlocks == 1 && k == 1)//1 byte, 6 && 7 bit
			{
				bit = 6;
				do{
					if ((block[k] >> bit)& 1)
						block[k] &= ~(1 << bit);
					else
						block[k] |= (1 << bit);
					++bit;
				}while(bit < 8);
			}
			//------------------------------------
			setWriteModeRS485();
			writeBufTX(block[k]);
		}
		_delay_ms(1);
		setWriteModeRS485();
		writeBufTX(crc);
	}
	
	return;
}

uint8_t calc_crc8_rohs(uint8_t *block, uint8_t size)
{
	uint8_t crc = 0xFF;
	while (size--)
		crc = pgm_read_byte(&(CRC_8_ROHS_table[crc ^ *block++]));
	return crc;
}

void print(char *str)//max length 255
{
	uint8_t i = 0;
	while(str[i] != '\0')
	{
		_delay_ms(1);
		setWriteModeRS485();
		writeBufTX(str[i]);
		++i;
	}
}