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
  - Requires changes to Api Gateway through aws portal (does not work out of the box)
    - Follow these steps https://stackoverflow.com/a/61752279/3159342

Here is a link to the build: https://awsaspcore.codari.co/

You will probably want this: https://aws.amazon.com/visualstudio/
