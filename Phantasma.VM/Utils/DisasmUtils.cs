﻿using System.Collections.Generic;
using System.Linq;

namespace Phantasma.VM.Utils
{
    public struct DisasmMethodCall
    {
        public string MethodName;

        // TODO method arguments
    }

    public static class DisasmUtils
    {
        public static IEnumerable<DisasmMethodCall> ExtractMethodCalls(byte[] script)
        {
            var disassembler = new Disassembler(script);
            return ExtractMethodCalls(disassembler);
        }

        public static IEnumerable<DisasmMethodCall> ExtractMethodCalls(Disassembler disassembler)
        {
            var instructions = disassembler.Instructions.ToArray();
            var result = new List<DisasmMethodCall>();

            int index = 0;
            var regs = new VMObject[16];
            while (index < instructions.Length)
            {
                var instruction = instructions[index];

                switch (instruction.Opcode)
                {
                    case Opcode.LOAD:
                        {
                            var dst = (byte)instruction.Args[0];
                            var type = (VMType)instruction.Args[1];
                            var bytes = (byte[])instruction.Args[2];

                            regs[dst] = new VMObject();
                            regs[dst].SetValue(bytes, type);

                            break;
                        }

                    case Opcode.EXTCALL:
                        {
                            var srcReg = (byte)instruction.Args[0];
                            result.Add(new DisasmMethodCall() { MethodName = regs[srcReg].AsString() });
                            break;
                        }
                }

                index++;
            }

            return result;
        }
    }
}
