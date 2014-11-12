QRSAPI_Manage
=============

Qlik Sense Repository API Windows Sample App
This app enables certain Qlik Sense server management capabilities to be
controlled using the Qlik Sense Repository (QRS) API.

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
1.  Code walk through!
2.  More functionality!
    a.  Manage data connections.
    b.  User provisioning to streams through Security Rules.
    c.  And so much more!
