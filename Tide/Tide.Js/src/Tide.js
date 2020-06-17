import "../node_modules/cryptide/dist/cryptide";

class Tide {
  constructor(orkNodes) {
    this.orkNodes = orkNodes || [];
  }

  countNodes() {
    return this.orkNodes.length;
  }

  register(username, password) {
    return new Promise(async function (resolve, reject) {
      // Some local validation, which is all we can really do.
      if (username.length < 3 || password.length < 6)
        return reject("Invalid credentials");

      // Attempt to fetch an MSA using the given username
      // If a user exists, check to see if we can login. If not, reject
      // If a user does not exist, create a new msa using the given credentials

      // Now that we have an msa, create a new cvk for this user/vendor
      // Return the keys

      resolve({ pub: "This is a public key", priv: "This is a private key" });
    });
  }

  login(username, password) {
    return new Promise(async function (resolve, reject) {
      resolve(true);
    });
  }

  forgotPassword(username) {
    // Trigger selected orks to send email fragments
  }

  combineForgetPasswordFragments(fragments) {}

  resetPassword(username, password) {}
}

export default Tide;
