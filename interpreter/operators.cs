namespace PeachInterpreter
{
    static class Operators
    {
        public readonly Dictionary<String, OperatorType> Map = new Dictionary<String, OperatorType>() {
            {"*", OperatorType.star },
            {"/", OperatorType.slash },
            {"+", OperatorType.plus },
            {"-", OperatorType.minus },
            {"%", OperatorType.perc },
            {"=", OperatorType.eq },
            {"==", OperatorType.eqeq },
            {"++", OperatorType.plusplus },
            {"--", OperatorType.minmin },
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
        plusplus,
        minmin,
        not,
        neq,
    }
}