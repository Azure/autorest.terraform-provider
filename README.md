# Azure Terraform Provider Generator (AutoRest Plugin)

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## AutoRest extension configuration

```yaml
use-extension:
    "@microsoft.azure/autorest.modeler": "2.3.47"

pipeline:
    terraform-provider/imodeler1:
        input: openapi-document/identity
        output-artifact: code-model-v1
        scope: terraform-provider
    terraform-provider/commonmarker:
        input: imodeler1
        output-artifact: code-model-v1
    terraform-provider/cm/transform:
        input: commonmarker
        output-artifact: code-model-v1
    terraform-provider/cm/emitter:
        input: transform
        scope: scope-cm/emitter
    terraform-provider/generate:
        plugin: terraform-provider
        input: cm/transform
        output: source-file-terraform-provider
    terraform-provider/transform:
        input: generate
        output-artifact: source-file-terraform-provider
        scope: scope-transform-string
    terraform-provider/emitter:
        input: transform
        scope: scope-terraform-provider/emitter

scope-terraform-provider/emitter:
    input-artifact: source-file-terraform-provider
    output-uri-expr: $key

output-artifact:
  - source-file-terraform-provider
```
