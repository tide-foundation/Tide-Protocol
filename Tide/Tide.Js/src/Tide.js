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
      if (username == "" || username.length < 3)
        return reject("Invalid username");
      if (password == "" || password.length < 6)
        return reject("Invalid password");
      resolve({ pub: "This is a public key", priv: "This is a private key" });
    });
  }

  login(username, password) {
    return new Promise(async function (resolve, reject) {
      resolve(true);
    });
  }
}

export default Tide;
