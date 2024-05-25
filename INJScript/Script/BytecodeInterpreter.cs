using System.Collections.Generic;
using System;

namespace Script
{
    public class BytecodeInterpreter
    {
        private readonly List<Instruction> _instructions;
        private readonly Stack<int> _stack;
        private readonly Dictionary<int, Action<BytecodeInterpreter>> _systemFunctions;
        private int _instructionPointer;

        public BytecodeInterpreter()
        {
            _instructions = new List<Instruction>();
            _stack = new Stack<int>();
            _systemFunctions = new Dictionary<int, Action<BytecodeInterpreter>>();
            _instructionPointer = 0;
        }

        public void LoadInstructions(List<Instruction> instructions)
        {
            _instructions.Clear();
            _instructions.AddRange(instructions);
        }

        public void RegisterSystemFunction(int id, Action<BytecodeInterpreter> function)
        {
            _systemFunctions[id] = function;
        }

        public void Execute()
        {
            while (_instructionPointer < _instructions.Count)
            {
                var instruction = _instructions[_instructionPointer];
                ExecuteInstruction(instruction);
            }
        }

        private void ExecuteInstruction(Instruction instruction)
        {
            switch (instruction.OpCode)
            {
                case OpCode.PUSH:
                    _stack.Push(instruction.Operand);
                    break;
                case OpCode.POP:
                    _stack.Pop();
                    break;
                case OpCode.ADD:
                    {
                        int b = _stack.Pop();
                        int a = _stack.Pop();
                        _stack.Push(a + b);
                        break;
                    }
                case OpCode.SUB:
                    {
                        int b = _stack.Pop();
                        int a = _stack.Pop();
                        _stack.Push(a - b);
                        break;
                    }
                case OpCode.MUL:
                    {
                        int b = _stack.Pop();
                        int a = _stack.Pop();
                        _stack.Push(a * b);
                        break;
                    }
                case OpCode.DIV:
                    {
                        int b = _stack.Pop();
                        int a = _stack.Pop();
                        _stack.Push(a / b);
                        break;
                    }
                case OpCode.SYS_CALL:
                    {
                        if (_systemFunctions.TryGetValue(instruction.Operand, out var function))
                        {
                            function(this);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Unknown system function ID: {instruction.Operand}");
                        }
                        break;
                    }
                case OpCode.JUMP:
                    _instructionPointer = instruction.Operand;
                    return; // Skip the normal increment of the instruction pointer
                case OpCode.JUMP_IF_ZERO:
                    if (_stack.Pop() == 0)
                    {
                        _instructionPointer = instruction.Operand;
                        return;
                    }
                    break;
                case OpCode.JUMP_IF_NEG:
                    if (_stack.Pop() < 0)
                    {
                        _instructionPointer = instruction.Operand;
                        return;
                    }
                    break;
                default:
                    throw new InvalidOperationException($"Unknown opcode: {instruction.OpCode}");
            }
            _instructionPointer++;
        }

        public int PeekStack()
        {
            return _stack.Peek();
        }
    }
}
