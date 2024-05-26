using System.Collections.Generic;
using System;
using System.Reflection.Emit;
using System.Linq;

namespace Script
{
    public class InjScrptInterpret
    {
        private readonly List<Instruction> _instructions;
        private readonly Stack<object> _stack;
        private readonly Dictionary<int, Action<InjScrptInterpret>> _systemFunctions;
        private readonly Dictionary<string, int> _labels;
        private int _instructionPointer;

        // other less important things (not needed to actually run with full functionality)
        private StateDebugger _state_debugger;

        public InjScrptInterpret()
        {
            _instructions = new List<Instruction>();
            _stack = new Stack<object>();
            _systemFunctions = new Dictionary<int, Action<InjScrptInterpret>>();
            _labels = new Dictionary<string, int>();
            _instructionPointer = -1;
        }

        public void LoadInstructions(List<Instruction> instructions)
        {
            _instructions.Clear(); // prepare for new instruction set
            _labels.Clear(); // prepare for new labels (if any)

            _instructions.AddRange(instructions); // add instructions to interpreter range

            _instructionPointer = 0; // reset instruction pointer

            // preprocess labels
            for (int i = 0; i < _instructions.Count; ++i)
            {
                if (_instructions[i].OpCode == OpCode.DEF_LABEL)
                {
                    _labels.Add((string)_instructions[i].Operand, i);
                }
            }
        }

        public void RegisterSystemFunction(int id, Action<InjScrptInterpret> function)
        {
            _systemFunctions[id] = function;
        }

        public void Halt()
        {
            _instructionPointer = -1;
        }

        public void Execute()
        {
            while (_instructionPointer < _instructions.Count && _instructionPointer != -1)
            {
                var instruction = _instructions[_instructionPointer];

                // debug step
                if (_state_debugger != null)
                {
                    _state_debugger.Trigger(instruction, _instructionPointer, _stack);
                }

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
                case OpCode.DUP:
                    {
                        int b = Convert.ToInt32(_stack.Peek());
                        _stack.Push(b);
                        break;
                    }
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
                    return; // Skip the normal increment of the IP (instruction pointer)
                case OpCode.JUMP_IF:
                    {
                        var comparer = Convert.ToInt32(_stack.Pop());
                        var result = Convert.ToInt32(_stack.Pop());

                        if (comparer == result)
                        {
                            _instructionPointer = Convert.ToInt32(instruction.Operand);
                            return;
                        }
                    }
                    break;
                case OpCode.JUMP_IF_GE:
                    {
                        var value = Convert.ToInt32(_stack.Pop());
                        var limit = Convert.ToInt32(_stack.Peek());
                        if (value >= limit)
                        {
                            _instructionPointer = Convert.ToInt32(instruction.Operand);
                            return;
                        }
                    }
                    break;
                case OpCode.JUMP_IF_NGE:
                    {
                        var value = Convert.ToInt32(_stack.Pop());
                        var limit = Convert.ToInt32(_stack.Peek());
                        if (value < limit)
                        {
                            _instructionPointer = Convert.ToInt32(instruction.Operand);
                            return;
                        }
                    }
                    break;
                case OpCode.JUMP_IF_NEG:
                    if (Convert.ToInt32(_stack.Pop()) < 0)
                    {
                        _instructionPointer = Convert.ToInt32(instruction.Operand);
                        return;
                    }
                    break;
                case OpCode.JUMP_LABEL_IF:
                    {
                        var value = Convert.ToInt32(_stack.Pop());
                        var limit = Convert.ToInt32(_stack.Peek());

                        if (value == limit)
                        {
                            _instructionPointer = _labels[(string)instruction.Operand];
                            return;
                        }
                    }
                    break;
                case OpCode.JUMP_LABEL_IF_GE:
                    {
                        var limit = Convert.ToInt32(_stack.Pop());
                        var value = Convert.ToInt32(_stack.Peek());

                        if (value >= limit)
                        {
                            _instructionPointer = _labels[(string)instruction.Operand];
                            return;
                        }
                    }
                    break;
                case OpCode.JUMP_LABEL_IF_NGE:
                    {
                        var limit = Convert.ToInt32(_stack.Pop());
                        var value = Convert.ToInt32(_stack.Peek());

                        if (value < limit)
                        {
                            _instructionPointer = _labels[(string)instruction.Operand];
                            return;
                        }
                    }
                    break;
                case OpCode.JUMP_LABEL_IF_NEG:
                    if (Convert.ToInt32(_stack.Pop()) < 0)
                    {
                        _instructionPointer = _labels[(string)instruction.Operand];
                        return;
                    }
                    break;
                case OpCode.JUMP_LABEL:
                    _instructionPointer = _labels[(string)instruction.Operand];
                    return; // skil the normal increment of the IP (instruction pointer)
                case OpCode.DEF_LABEL:
                    // we reprocess labels before executing the instructions
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

        public void Push(object obj)
        {
            _stack.Push(obj);
        }

        public void Attach(StateDebugger state_debugger)
        {
            _state_debugger = state_debugger;
        }
    }
}
