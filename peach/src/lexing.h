#ifndef TOK_HEADER
#define TOK_HEADER
	#include "tok.h"
#endif
#ifndef LOG_HEADER
#define LOG_HEADER
	#include "log.h"
#endif

tok *readline(char *line_buffer, char *lexeme, int loc, int *cnt);
enum ttype get_ttype(char *lex, int row, int col);
enum ttype get_literal_type(char *lex, int row, int col);
tok *add_token(tok *toks, int *tok_cnt, int row, int col, char *lex);
int is_number(char *str, int len);

void *get_val_by_ttype(enum ttype t, char *lex);

typedef struct toklib_el
{
       char *lex;
       enum ttype type;
} toklib_el;

#define keywords_len 9
const toklib_el keywords[keywords_len] = {
       {.lex = "+", .type = op_add},
       {.lex = "-", .type = op_sub},
       {.lex = "/", .type = op_div},
       {.lex = "*", .type = op_mul},
       {.lex = "::", .type = key_map},
       {.lex = ":<", .type = key_lfold},
       {.lex = ":>", .type = key_rfold},
       {.lex = ":!", .type = key_filter},
       {.lex = "~", .type = key_avg}};

tok *readline(char *line_buffer, char *lexeme, int loc, int *cnt)
{
	tok *toks = (tok *)malloc(sizeof(tok));
	int cur_lex_start = 0;
	bool in_stringlit = false;
	memset(lexeme, 0, strlen(lexeme));
	
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
				cur_lex_start = i + 1;
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
			throw_error_quit(lexeme, loc, cur_lex_start, "Error: unclosed string literal.");
		else
			toks = add_token(toks, cnt, loc, cur_lex_start, lexeme);

	free(lexeme);
	return toks;
}

tok *add_token(tok *toks, int *tok_cnt, int row, int col, char *lex)
{
	char *tval = malloc(sizeof(lex));
	strcpy(tval, lex);
	enum ttype type = get_ttype(lex, row, col);
	toks[*tok_cnt] = (tok){.col = col, .type = type, .val_uncasted = tval, .val = get_val_by_ttype(type, tval)};
	(*tok_cnt)++;
	memset(lex, 0, strlen(lex));
	return (tok *)realloc(toks, ((*tok_cnt) + 1) * sizeof(tok));
}

void *get_val_by_ttype(enum ttype t, char *lex) {
	void *out;

	switch(t) {
		case lit_int:
			int cast_i = atoi(lex);
			out = (void *)&cast_i;
			break;

		case lit_string:
			out = lex;
			break;

		case lit_float:
			float cast_f = atof(lex);
			out = (void *)&cast_f;
			break;

		default:
			break;
	};

	return out;
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
