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
    }
}