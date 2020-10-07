# Registration

Once you have your [TideAuthentication](linkToAPI) instance by following the steps outlined [here](linkToAPI), you're ready to start onboarding users.

```javascript
// The username of the new user
const username = "newUser";

// The password of the new user
const password = "pa$$w0rd";

// The email used for recovery
const email = "newUser@example.com";

// An array of ork nodes selected by the user to use to register their account
const orkUrls = ["https://ork-1.com", "https://ork-2.com", "https://ork-3.com"];

// vanilla JS
auth
  .register(username, password, email, orkUrls)
  .then((account) => {
    // Used to interact with your Tide account in the context of this vendor
    const vendorKey = account.vendorKey;

    // This users unique vendor ID.
    // This can be used to link the user to your own backend account system.
    const vuid = account.vuid;
  })
  .catch((e) => console.log(e));

// ES6
const account = await auth.register(username, password, email, orkUrls);
```
