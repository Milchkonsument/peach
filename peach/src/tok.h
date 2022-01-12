enum ttype
{
        op_add,
	op_sub,
	op_div,
	op_mul,

	key_map,
	key_lfold,
	key_rfold,
	key_filter,
	key_avg,
        
        lit_int,
        lit_float,
        lit_string,
        
        identifier,
        none
};

const char *ttype_to_str[14] = {
	"op:add",
	"op:sub",
	"op:mul",
	"op:div",

	"key:map",
	"key:lfold",
	"key:rfold",
	"key:filter",
	"key:avg",

	"lit:int",
	"lit:float",
	"lit:string",

       "identif",
       "-none-"};

typedef struct tok
{
        enum ttype type;
        char *val_uncasted;
	void *val;
        int col;
} tok;

