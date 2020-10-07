# Password Management

## Change Password

```javascript
// The current user password
const password = "pa$$w0rd";

// The new user password
const newPassword = "pa$$w0rd2";

// vanilla JS
auth
  .changePassword(password, newPassword)
  .then(() => {
    // Success
  })
  .catch((e) => console.log(e));

// ES6
await auth.changePassword(password, newPassword); // Throw if failed
```

## Forgot Password

Tide offers great security benefits over a traditional authentication system with almost no negetive effects on user experience, at the expense of the 'Forgot Password' scenario. Tide provides tools to make this experience as streamlined as possible. (ew, somebody re-write this)

### Step 1

```javascript
// The username of the Tide account which requires a new password
const username = "existingTideUsername";

// vanilla JS
auth
  .recover(username)
  .then(() => {
    // Success. The orks used to create this users account have been notified. Each ork node
    // will send the recovery email assosiated with this account a recovery fragment.
  })
  .catch((e) => console.log(e));

// ES6
await auth.recover(username); // Throw if failed
```

### Step 2

Once the user recovers the shards from the recovery email(s), they can then be used to reconstruct the users password and assign a new one.

```javascript
// The username of the Tide account which requires a new password
const username = "existingTideUsername";

// The shared gathered from the users recovery email(s)
const shares = ["share1", "share2", "share3", "etc"];

// The password the user wishes to use
const newPassword = "existingTideUsername";

// vanilla JS
auth
  .reconstruct(username, shares, newPassword)
  .then((account) => {
    // The user has recovered their password and now has access once again.
  })
  .catch((e) => console.log(e));

// ES6
await auth.reconstruct(username, shares, newPassword); // Throw if failed
```
