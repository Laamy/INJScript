using System.Collections.Generic;
using System;

namespace Script
{
    public class BytecodeInterpreter
    {
        private readonly List<Instruction> _instructions;
        private readonly Stack<object> _stack;
        private readonly Dictionary<int, Action<BytecodeInterpreter>> _systemFunctions;
        private int _instructionPointer;

        public BytecodeInterpreter()
        {
            _instructions = new List<Instruction>();
            _stack = new Stack<object>();
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
                case OpCode.PUSH_INT:
                    _stack.Push(instruction.Operand);
                    break;
                case OpCode.PUSH_STRING:
                    _stack.Push(instruction.Operand);
                    break;
                case OpCode.POP:
                    _stack.Pop();
                    break;
                case OpCode.ADD:
                    {
                        int b = Convert.ToInt32(_stack.Pop());
                        int a = Convert.ToInt32(_stack.Pop());
                        _stack.Push(a + b);
                        break;
                    }
                case OpCode.SUB:
                    {
                        int b = Convert.ToInt32(_stack.Pop());
                        int a = Convert.ToInt32(_stack.Pop());
                        _stack.Push(a - b);
                        break;
                    }
                case OpCode.MUL:
                    {
                        int b = Convert.ToInt32(_stack.Pop());
                        int a = Convert.ToInt32(_stack.Pop());
                        _stack.Push(a * b);
                        break;
                    }
                case OpCode.DIV:
                    {
                        int b = Convert.ToInt32(_stack.Pop());
                        int a = Convert.ToInt32(_stack.Pop());
                        _stack.Push(a / b);
                        break;
                    }
                case OpCode.CONCAT:
                    {
                        var b = _stack.Pop();
                        var a = _stack.Pop();
                        _stack.Push(a.ToString() + b.ToString());
                        break;
                    }
                case OpCode.SYS_CALL:
                    {
                        if (_systemFunctions.TryGetValue(Convert.ToInt32(instruction.Operand), out var function))
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
                    _instructionPointer = Convert.ToInt32(instruction.Operand);
                    return; // Skip the normal increment of the instruction pointer
                case OpCode.JUMP_IF_ZERO:
                    if (Convert.ToInt32(_stack.Pop()) == 0)
                    {
                        _instructionPointer = Convert.ToInt32(instruction.Operand);
                        return;
                    }
                    break;
                case OpCode.JUMP_IF_NEG:
                    if (Convert.ToInt32(_stack.Pop()) < 0)
                    {
                        _instructionPointer = Convert.ToInt32(instruction.Operand);
                        return;
                    }
                    break;
                default:
                    throw new InvalidOperationException($"Unknown opcode: {instruction.OpCode}");
            }
            _instructionPointer++;
        }

        public object PeekStack()
        {
            return _stack.Peek();
        }
    }
}
