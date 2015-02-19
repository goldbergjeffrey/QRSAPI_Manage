QRSAPI_Manage
=============
Qlik Sense Repository API Windows Sample App with a little .Net SDK for good measure.
This app enables certain Qlik Sense server management capabilities to be
controlled using the Qlik Sense Repository (QRS) API.

In addition, allows for primitive app bundling from Sense Desktop to Sense Server.  Primitive in that only the app and associated images that do not already exist on the Sense Server will be migrated.

REQUIREMENTS
=============
In order to use QRSAPI_MANAGE the following requirements are necessary:

1.  Export certificates from the Qlik Sense Server and import the root certificate into the trusted root certificate folder of the local computer certificate store.

2.  A virtual proxy with header injection needs to be created prior to using this application.  To create a virtual proxy with header injection, please review the document located here: https://github.com/goldbergjeffrey/QRSAPI_Manage/blob/master/QlikSense_SetupHeaderAuthVirtualProxy.pdf


Update! 18-FEB-2015
=================================================================
- New look with logo on top and tab control instead of menu buttons.

- Removed connection editor button as this capability has not been activated.

- Users can upload app and images from Qlik Sense desktop to Qlik Sense Server in one upload process.

Original Release
=================================================================

The goal is to provide examples of how the QRS API works, how to call
out to the API for information, and make changes to a Qlik Sense
configuration.

Capabilities in first commit:
1.  Upload an app from a local system (likely running Qlik Sense
Desktop) to Qlik Sense Server.

2.  Upload content to an existing content library.

3.  Publish an app to a stream.

4.  Create a reload task and the appropriate trigger to refresh data in
an app.

PREREQUISITES:
1.  Licensed Qlik Sense server installation.

2.  Establish a virtual proxy using header authorization (config
document included).

COMING SOON!
=================================================================
1.  Code walk through!

2.  More functionality!
    a.  Manage data connections.

    b.  User provisioning to streams through Security Rules.

    c.  And so much more!
