using System.Collections.Generic;
using System;

namespace PeachInterpreter
{
    static class Operators
    {
        public static readonly Dictionary<string, OperatorType> Map = new Dictionary<string, OperatorType>() {
            {"*", OperatorType.star },
            {"/", OperatorType.slash },
            {"+", OperatorType.plus },
            {"-", OperatorType.minus },
            {"%", OperatorType.perc },
            {"=", OperatorType.eq },
            {"==", OperatorType.eqeq },
            {"++", OperatorType.plpl },
            {"--", OperatorType.mnmn },
            {"!", OperatorType.not },
            {"!=", OperatorType.neq },
            {"<", OperatorType.lt },
            {">", OperatorType.gt },
            {"<=", OperatorType.lteq },
            {">=", OperatorType.gteq },
            {"~", OperatorType.mut },
        };
    }

    enum OperatorType
    {
        star,
        slash,
        plus,
        minus,
        perc,
        eq,
        eqeq,
        plpl,
        mnmn,
        not,
        neq,
        lt,
        gt,
        lteq,
        gteq,
        mut,
    }
}