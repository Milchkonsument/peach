#include <stdio.h>
#include <ctype.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>

enum ttype
{
	key_int,
	key_print,
	key_float,
	key_string,

	op_add,
	op_ass,

	lit_int,
	lit_float,
	lit_string,

	identifier,
	none
};
const char *ttype_to_str[9] = {
	"key:int",
	"key:print",
	"key:float",
	"key:string",

	"opr:add",
	"opr:ass",

	"lit:int",
	"lit:float",
	"lit:string",

	"identif",
	"-none-"};

typedef struct toklib_el
{
	char *lex;
	enum ttype type;
} toklib_el;

#define keywords_len 6
const toklib_el keywords[keywords_len] = {
	{.lex = "+", .type = op_add},
	{.lex = "=", .type = op_ass},

	{.lex = "int", .type = key_int},
	{.lex = "float", .type = key_float},
	{.lex = "string", .type = key_string},
	{.lex = "print", .type = key_print}};

int main(int argc, char *argv[]);
tok *readline(char *line_buffer, int loc, int *cnt);
enum ttype get_ttype(char *lex, int row, int col);
enum ttype get_literal_type(char *lex, int row, int col);
tok *add_token(tok *toks, int *tok_cnt, int row, int col, char *lex);
int is_number(char *str, int len);
throw_error_quit(char *lex, int row, int col, char *msg);

int main(int argc, char *argv[])
{
	if (argc < 2)
	{
		printf("provide a file");
		return 1;
	}

	FILE *f = fopen(argv[1], "r");
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
		if (strlen(buff) == 0 || buff[0] == '\n' || buff[0] == '\0' || buff[0] == '#')
			continue;

		int cnt = 0;
		tok *toks = readline(buff, loc, &cnt);

		for (size_t i = 0; i < cnt; i++)
		{
			printf("[%02d,%02d] [%s]\t'%s'\n", toks[i].row, toks[i].col, ttype_to_str[toks[i].type], toks[i].val_uncasted);
		}

		free(toks);
	}

	free(f);
}

tok *readline(char *line_buffer, int loc, int *cnt)
{
	tok *toks = (tok *)malloc(sizeof(tok));
	char *lexeme = (char *)malloc(sizeof(char));
	int cur_lex_start = 0;
	bool in_stringlit = false;
	
	for (size_t i = 0; i < strlen(line_buffer); i++)
	{
		char c = line_buffer[i];

		if(c == '\"' || c == '\'')
			in_stringlit = !in_stringlit;

		if (isspace(c) && !in_stringlit)
		{
			if (strlen(lexeme) != 0)
			{
				toks = add_token(toks, cnt, loc, cur_lex_start, lexeme);
				cur_lex_start = (*cnt) + 1;
			}

			continue;
		}

		int len = strlen(lexeme);
		lexeme[len] = c;
		lexeme = realloc(lexeme, (len + 1) * sizeof(char));
	}

	// clear out lexeme content still left til EOF
	if (strlen(lexeme) != 0)
		if(in_stringlit)
			throw_error_quit(lexeme, loc, cnt, "Error: unclosed string literal.");
		else
			toks = add_token(toks, cnt, loc, cur_lex_start, lexeme);

	free(lexeme);

	return toks;
}

tok *add_token(tok *toks, int *tok_cnt, int row, int col, char *lex)
{
	char *tval = malloc(sizeof(lex));
	strcpy(tval, lex);
	toks[*tok_cnt] = (tok){.row = row, .col = col, .type = get_ttype(lex, row, col), .val_uncasted = tval};
	(*tok_cnt)++;
	memset(lex, 0, strlen(lex));
	return (tok *)realloc(toks, ((*tok_cnt) + 1) * sizeof(tok));
}

enum ttype get_ttype(char *lex, int row, int col)
{
	enum ttype out = none;

	for (size_t i = 0; i < keywords_len; i++)
	{
		if (strcmp(keywords[i].lex, lex) == 0)
		{
			out = keywords[i].type;
			break;
		}
	}

	if (out == none)
		out = get_literal_type(lex, row, col);

	return out;
}

enum ttype get_literal_type(char *lex, int row, int col)
{
	size_t len = strlen(lex);

	if((lex[0] == '\'' && lex[len-1] == '\'') || (lex[0] == '\"' && lex[len-1] == '\"'))
		return lit_string;

	int ret = is_number(lex, len);
	if(ret == 1)
		return lit_int;
	if(ret == 2)
		return lit_float;
	if(ret != 0)
		throw_error_quit(lex, row, col, "Parse error for number literal.");

	return identifier;
}

int is_number(char *str, int len) {
	bool floatpt = false;
	for(int i = 0; i < len; i++)
	{
		if(!isdigit(str[i])) {
			if(str[i] == '.') {
				if(floatpt) {
					return -1;
				} else {
					floatpt = true;
				}
				return len > i ? 2 : -2;
			} else {
				return 0;
			}
		}
	}
	return 1;
}

throw_error_quit(char *lex, int row, int col, char *msg) {
	printf("Error at [%i,%i] at '%s': %s", row, col, lex, msg);
	exit(-1);
}
