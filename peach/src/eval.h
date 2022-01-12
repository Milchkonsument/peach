#include <stdbool.h>

#ifndef TOK_HEADER
#define TOK_HEADER
	#include "tok.h"
#endif
#ifndef LOG_HEADER
#define LOG_HEADER
	#include "log.h"
#endif
#ifndef LEX_HEADER
#define LEX_HEADER
	#include "lexing.h"
#endif

tok *sym_table;

// global vars //
int i = 0;
int cnt = 0;
tok *toks;
// global vars //

void eval(tok *toks_, int cnt_);
void eval_exp(tok *tok);
int eval_exp_int();
float eval_exp_float();
char *eval_exp_string();
void eval_decl_int(tok *iden, tok *eq, tok *exp_int);
void eval_decl_float(tok *val_type, tok *iden, tok *eq);
void eval_decl_string(tok *val_type, tok *iden, tok *eq);
void eval_op_print(tok *val);
void eval_op_add();
void eval_op_sub();
void eval_op_div();
void eval_op_mul();
void eval_op_ass();

tok *current();
tok *next();
tok *consume();
tok *peek();
bool peek_next(char *lex);
bool peek_next_type(enum ttype t);
tok *peek_n(int n);
bool at_end();
bool suff_args(int n);

void add_symbol(tok *itm);
tok *get_symbol_val(char *id);

int calc_hash(char *id);
void err_expected(char *expected);

// eval functions //
void eval(tok *toks_, int cnt_) 
{
	sym_table = malloc(512 * sizeof(tok));
	i = 0;
	cnt = cnt_;
	toks = toks_;

	eval_exp(consume());
	free(sym_table);
}

void eval_exp(tok *tok) {
	switch(tok->type) {
		default:
			throw_error_quit(tok->val_uncasted, 0, tok->col, "Unhandled token type.");
	};
}

int eval_exp_int(tok *val) {
	// this will give erroneous results due to falsely assuming right associativity for everything and ignoring operator precedence
	// ..but its good enough for now.
	if(peek_next("+")) {
		int lhs = atoi(val->val_uncasted);
		int rhs = eval_exp_int(consume());
		int sum = lhs + rhs;
		return sum;
	}

	if(peek_next("-"))
		return atoi(val->val_uncasted) - eval_exp_int(consume());
	
	if(peek_next("*"))
		return atoi(val->val_uncasted) * eval_exp_int(consume());
	
	if(peek_next("/"))
		return atoi(val->val_uncasted) / eval_exp_int(consume());

	if(!is_number(val->val_uncasted, strlen(val->val_uncasted)))
		err_expected("expected int.");

	return atoi(val->val_uncasted);
}

float eval_exp_float(tok *val) {
	//TODO
	return 0.0f;
}

char *eval_exp_string(tok *val) {
	//TODO: check for string concatenation
	return val->val_uncasted;
}

void eval_decl_int(tok *iden, tok *eq, tok *exp_int) {
	if(iden->type != identifier)
		err_expected("expected identifier.");
	
	if(iden->val_uncasted[0] != '_' && !isalpha(iden->val_uncasted[0]))
		err_expected("expected identifier meeting naming convention requirements.");

	int e = eval_exp_int(exp_int);
	void *ptr = &e;
	tok sym = (tok){.type = lit_int, .val = ptr, .val_uncasted = iden->val_uncasted};
	add_symbol(&sym);
}

void eval_decl_float(tok *val_type, tok *iden, tok *val) {
	//TODO
}

void eval_decl_string(tok *val_type, tok *iden, tok *val) {
	//TODO
}

void eval_op_print(tok *val) {
	switch(val->type) {
		case lit_int:
			printf("%i\n", *((int *)val->val));
			break;
		case lit_float:
			printf("%f\n", *((float *)val->val));
			break;
		case lit_string:
			printf((char *)val->val);
			break;
		case identifier: 
			eval_op_print(get_symbol_val(val->val_uncasted));
			break;
		default:
			err_expected("expected identifier, literal value.");
	};
	
	printf(val->val_uncasted);
}

// end eval functions //
void add_symbol(tok *itm) {
	int hash = calc_hash(itm->val_uncasted);
	sym_table[hash] = *itm;
	printf("(%s / %s / %s) ->> [%04x]\n", itm->val_uncasted, itm->val, ttype_to_str[itm->type], hash);
}

tok *get_symbol_val(char *id) {
	for(int i = 0; i < 512; i++)
		printf("[%04x] | %s\n", i, sym_table[i].val_uncasted);
	
	int hash = calc_hash(id);
	tok *sym = &sym_table[hash];
	
	printf("(%s / %s) <- [%04x]\n", sym->val_uncasted, ttype_to_str[sym->type], hash);

	return sym;
}

int calc_hash(char *id) {
	int c = strlen(id);
	int p = 9133;
	int sum = 0;
	for(int i = 0; i < c; i++)
		sum += id[i];

	int adr = (sum * p) % 512;
	printf("calc_hash: %s -> %04x\n", id, adr);
	return adr;
}

bool suff_args(int n) {
	return i+n <= cnt;
}

tok *current() {
	return &toks[i];
}

tok *peek() {
	return &toks[i+1];
}

tok *peek_n(int n) {
	return &toks[i+n];
}

tok *consume() {
	return &toks[i++];
}

tok *next() {
	return &toks[++i];
}

bool peek_next(char *lex) {
	if(at_end())
		return false;
	
	if(strcmp(toks[i].val_uncasted, lex) == 0) {
		i++;
		return true;
	}

	return false;
}

bool peek_next_type(enum ttype t) {
	return !at_end() && toks[i+1].type == t ? i == i++ : false;
}

bool at_end() {
	return i >= cnt;
}

void err_expected(char *expected) {
	throw_error_quit(current()->val_uncasted, 0, current()->col, expected);
}
