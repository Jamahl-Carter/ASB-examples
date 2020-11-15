# Managing Syncronous Integrations with Azure Service Bus
Example implementation from this [article](https://medium.com/@carterjamahl/adopting-async-messaging-with-azure-service-bus-4c936396b334), operating a async workflow within a sync context.
## Quick start
0. Clone this repository `git clone https://github.com/Jamahl-Carter/ASB-examples.git`
1. Set-up Azure resources. 
    - Navigate to ./terraform folder in powershell.
    - `az login`
    - Ensure appropriate subscription is set: `az account set --subscription="subscription ID"`, to see current subscription use `az account show`.
    - Initilise working dir: `terraform init`
    - Generate/review terraform plan: `terraform plan -out plan.tfplan`
    - Create Azure resources: `terraform apply plan.tfplan`
2. Configuring application
    - Update .env file endpoint value to match newly created service bus namespace. You can use `terraform show -json` to see relevant Azure resource details, or login through Azure portal.
    - Update .env SAS key value to key from shared access policy.
3. Running application
    - Start application: `docker-compose up --scale sync-adapter.consumer={int: no. of consumer instances}`
4. Using the application
    - Find port sync-adapter.producer service is listening to. Can be found using `docker ps`
    - Send batch request `curl http://localhost:{int: port number}/api/test/batch/{int: no. to requests to send}`
    - Send single request `curl http://localhost:{int: port number}/api/test/{bool: Wait for response?}`
5. Clean-up Azure resources.
    - `terraform destroy`

## Tooling
- Azure CLI
- Terraform CLI
- Docker
- Visual Studio 2019 IDE (optional)

## License
MIT