using AutoRest.Core;
using AutoRest.Terraform.Templates;
using System.Collections.Generic;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    public class CreateGenerator
        : TfFunctionGeneratorBase
    {
        public CreateGenerator() => Singleton<CreateGenerator>.Instance = this;
        protected override ITemplate CreateTemplateCore() => new FunctionCallTemplate { Model = this };

        protected override string FunctionNamePostfix => "Create";
        public override IEnumerable<GoSDKInvocation> Invocations => CodeModel.CreateInvocations;
    }

    public class ReadGenerator
        : TfFunctionGeneratorBase
    {
        public ReadGenerator() => Singleton<ReadGenerator>.Instance = this;
        protected override ITemplate CreateTemplateCore() => new FunctionCallTemplate { Model = this };

        protected override string FunctionNamePostfix => "Read";
        public override IEnumerable<GoSDKInvocation> Invocations => CodeModel.ReadInvocations;
    }

    public class UpdateGenerator
        : TfFunctionGeneratorBase
    {
        public UpdateGenerator() => Singleton<UpdateGenerator>.Instance = this;
        protected override ITemplate CreateTemplateCore() => new FunctionCallTemplate { Model = this };

        protected override string FunctionNamePostfix => "Update";
        public override IEnumerable<GoSDKInvocation> Invocations => CodeModel.UpdateInvocations;
    }

    public class DeleteGenerator
        : TfFunctionGeneratorBase
    {
        public DeleteGenerator() => Singleton<DeleteGenerator>.Instance = this;
        protected override ITemplate CreateTemplateCore() => new FunctionCallTemplate { Model = this };

        protected override string FunctionNamePostfix => "Delete";
        public override IEnumerable<GoSDKInvocation> Invocations => CodeModel.DeleteInvocations;
    }
}
