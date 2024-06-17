using static Item;
using System;

[Serializable]
public class Operand
{
    public Operation operation;
    public float modifier;

    public Operand(Operation operation, float modifier)
    {
        this.operation = operation;
        this.modifier = modifier;
    }

    public override string ToString()
    {
        string pre = "";
        switch(operation)
        {
            case Operation.Additive:
                pre = " + ";
                break;
            case Operation.Multiplicative:
                pre = " * ";
                break;
            default:
                pre = "";
                break;
        }

        return pre + modifier;
    }
}