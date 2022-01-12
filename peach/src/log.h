void throw_error_quit(char *lex, int row, int col, char *msg);


void throw_error_quit(char *lex, int row, int col, char *msg) {
	printf("Error at [%i,%i] at '%s': %s", row, col, lex, msg);
	exit(-1);
}
