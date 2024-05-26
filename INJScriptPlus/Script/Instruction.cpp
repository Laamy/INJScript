#include "Instruction.h"
#include "iOpcode.h"

#include <any>

Script::Instruction::Instruction(iOpcode OpCode, std::any Operand = NULL) {
	this->OpCode = OpCode;
	this->Operand = Operand;
};