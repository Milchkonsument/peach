#include <stdio.h>
#include <ctype.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>
#include "eval.h"

#ifndef LEX_HEADER
#define LEX_HEADER
	#include "lexing.h"
#endif

int main(int argc, char *argv[]);

int main(int argc, char *argv[])
{
	if (argc < 3)
	{
		printf("usage: peach \"commands\" input");
		return 1;
	}
	
	if(argc == 3) {
		FILE *f = fopen(argv[2], "r");
		int buf_len = 255;
		char buff[buf_len];
		int loc = 0;

		if (!f)
		{
			printf("could not open file: %s", argv[2]);
			return 2;
		}

		while (fgets(buff, buf_len, f))
		{
			loc++;
			if (strlen(buff) == 0 || buff[0] == '\n' || buff[0] == '\0')
				continue;
			
			int cnt = 0;
			char *lex = malloc(sizeof(char));
			tok *toks = readline(buff, lex, loc, &cnt);
			 
			// eval(toks, cnt);
			free(toks);
		}

		free(f);
	} else {
		char *input[argc-1];
		for(int i = 0; i < argc-1; i++) {
			input[i] = argv[i];
		}

		for(int i = 0; i < argc-1; i++) {
			printf("-> %s", input[i]);
		}
	}
	return 0;
}
