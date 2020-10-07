# Login

Logging in is as easy as any traditional authentication system, a username/password is all that's required. Tide will perform a lookup for the required ork nodes automatically.

```javascript
// The username of the existing user
const username = "newUser";

// The password of the existing user
const password = "pa$$w0rd";

// vanilla JS
auth
  .login(username, password)
  .then((account) => {
    // Used to interact with your Tide account in the context of this vendor
    const vendorKey = account.vendorKey;

    // This users unique vendor ID.
    // This can be used to link the user to your own backend account system.
    const vuid = account.vuid;
  })
  .catch((e) => console.log(e));

// ES6
const account = await auth.login(username, password);
```
