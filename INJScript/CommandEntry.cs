using Script;
using System;
using System.Collections.Generic;

public class CommandEntry
{
    static void Main(string[] args)
    {
        var interpreter = new BytecodeInterpreter();

        // Define some instructions
        var instructions = new List<Instruction>
        {
            // push 1 & 2 onto the stack then add them together (1) with (2)
            new Instruction(OpCode.PUSH_INT, 1),
            new Instruction(OpCode.PUSH_INT, 2),
            new Instruction(OpCode.ADD),

            // push the string of " is the result" and combine the maths from earlier with it
            new Instruction(OpCode.PUSH_STRING, " is the result"),
            new Instruction(OpCode.CONCAT),

            // print the current thing ontop of the stack (in this cast it would be "3 is the result"
            new Instruction(OpCode.SYS_CALL, 0x10),

            // restart loop from bytecode 0 (while loop)
            new Instruction(OpCode.JUMP, 0)
        };

        // Load instructions into the interpreter
        interpreter.LoadInstructions(instructions);

        // Register system functions
        interpreter.RegisterSystemFunction(0x10, interp => {
            Console.WriteLine(interp.PeekStack());
        });

        // Execute the bytecode
        interpreter.Execute();

        Console.ReadKey();
    }
}