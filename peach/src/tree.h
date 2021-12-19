enum tflag {
	unr,
	bin
	prt
}

typedef struct tok
{
        enum ttype type;   
        char *val_uncasted;
        int row;
        int col;
} tok;

typedef struct prog
{
	exp_con *exps;
	size_t s_exps;
} exp;

typedef struct exp_con {
	tflag type;
	void *cont;
} exp_con;

typedef struct exp_un {
	exp_con l;
	exp_con op;
	exp_con r;
} exp_un;
