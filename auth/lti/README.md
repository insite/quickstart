# SSO LTI Demo

This repository contains the source code for a demonstration of Single Sign On (SSO) using Learning Tools Interoperability (LTI).

The integration endpoint for an LTI Launch in Shift iQ is platform-agnostic, therefore it can be used to integrate any application that is able to send an HTTP request. 

This particular demo shows how to implement SSO from a Microsoft ASP.NET web application. You'll notice the integration itself is a simple HTTP POST, so you can use the source code here as a reference for building an integration from any web application developed using any programming language.

## Setup Instructions

If you want to configure this demo web application in your local development environment, then you can follow the steps here.

First, clone the SsoLtiDemo repository to your local environment. 

Next, create an IIS Site that binds to the Source folder. For example, here you can see my site binds to `C:\Base\Repos\InSite\SsoLtiDemo\Source`:

![SsoLtiDemo-1](https://github.com/user-attachments/assets/0c20375b-721f-4012-bf96-bf307e2d4a71)

Remember to ensure your local **hosts** file includes an entry for the domain name you decide to use. The first two lines in my `C:\Windows\System32\drivers\etc\hosts` file look like this:

```
127.0.0.1 localhost
127.0.0.1 lti-launch-demo.insite.com
```

Edit the project file `LtiLaunch.csproj` and ensure the **IISUrl** element matches the domain name you decide to use.

Build and run the application. Fill in the LTI Launch Request form. (You will need an Organization Identifier and an Organization Secret from the InSite support team.)

![SsoLtiDemo-2](https://github.com/user-attachments/assets/7dd1be89-d4b4-4708-bef0-62d27df63b15)

Click the button **Validate and Launch**. This will create and display an HTTP POST request for your review. Read and understand the code behind this step so you know exactly what is required in the LTI launch.

![image](https://github.com/user-attachments/assets/3650bc8d-8df2-4d4a-839b-879771278d4c)

Click the link **Launch** to submit the HTTP POST request to the LTI endpoint in Shift iQ. If your request is valid, then you will be authenticated automatically by Shift iQ. If the learner identified in the request does not already exist in Shift iQ, then a new account for the learner is created and approved automatically.
