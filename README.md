# AwsAspCore
Example of AspCore in Aws

- Serverless, all ran in lambda (costing nothing to host for me atm)
- Movie list powered by Dynamo DB
- WebGL game imbeded with iframe
  - WebGL Game hosted in S3 bucket and given https status through amazon cloud front
  - WebGL Game reacts to url params
  - WebGL Project: https://github.com/Soren025/WebGLTest
- File Uploading
  - Image Uploads to S3
  - All images in said S3 displayed
  - Button to delete all the images

Here is a link to the build: https://awsaspcore.codari.co/
Not included in the project is my domain and domain certificate, but you can eaisly link a custom domain

You will probably want this: https://aws.amazon.com/visualstudio/
