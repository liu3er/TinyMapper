﻿using System;
using System.Reflection.Emit;
using TinyMapper.CodeGenerators;
using TinyMapper.CodeGenerators.Ast;
using TinyMapper.Extensions;

namespace TinyMapper.Builders.Assemblies.Types.Methods
{
    internal sealed class CreateInstanceMethodBuilder : EmitMethodBuilder
    {
        public CreateInstanceMethodBuilder(Type sourceType, Type targetType, TypeBuilder typeBuilder)
            : base(sourceType, targetType, typeBuilder)
        {
        }

        protected override void BuildCore()
        {
            EmitMethod(_targetType);
        }

        protected override MethodBuilder CreateMethodBuilder(TypeBuilder typeBuilder)
        {
            return typeBuilder.DefineMethod(ObjectTypeBuilder.CreateTargetInstanceMethodName,
                MethodAttribute, typeof(object), Type.EmptyTypes);
        }

        private IAstType CreateRefType(Type type)
        {
            return type.HasDefaultCtor() ? AstNewObj.NewObj(type) : AstLoadNull.Load();
        }

        private IAstType CreateValueType(Type type, CodeGenerator codeGenerator)
        {
            LocalBuilder builder = codeGenerator.DeclareLocal(type);
            AstLocalVariableDeclaration.Declare(builder).Emit(codeGenerator);
            return AstBox.Box(AstLoadLocal.Load(builder));
        }

        private void EmitMethod(Type type)
        {
            IAstType value = type.IsValueType ? CreateValueType(type, _codeGenerator) : CreateRefType(type);
            AstReturn.Return(type, value).Emit(_codeGenerator);
        }
    }
}
