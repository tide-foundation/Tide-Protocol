import DAuthFlow from "./dauth/DAuthFlow";
import IdGenerator from "./IdGenerator";
import request from "superagent";
import { encodeBase64Url } from "./Helpers";

class Tide {
  constructor(vendorId, serverUrl) {
    this.vendorId = vendorId;
    this.serverUrl = serverUrl;
  }

  register(username, password, email, orkIds) {
    return new Promise(async (resolve, reject) => {
      try {
        // Some local validation, which is all we can really do.
        if (username.length < 3 || password.length < 6)
          return reject("Invalid credentials");

        var flow = new DAuthFlow(generateOrkUrls(orkIds), username);
        var userId = encodeBase64Url(new IdGenerator(username).buffer);
        // Ask the vendor to create the user as a liability.

        await post(`${this.serverUrl}/CreateUser/${userId}`, orkIds);
        // Send all shards to selected orks
        var key = await flow.signUp(password, email, 2);
        // Finally, ask the vendor to confirm the user
        await get(`${this.serverUrl}/ConfirmUser/${userId}/`);

        this.key = key;
        resolve({ key: key });
      } catch (error) {
        reject(error);
        // await get(`${this.serverUrl}/RollbackUser/${userId}/`);
      }
    });
  }
  login(username, password) {
    return new Promise(async (resolve, reject) => {
      try {
        var userId = encodeBase64Url(new IdGenerator(username).buffer);
        var userNodes = JSON.parse(
          await get(`${this.serverUrl}/GetUserNodes/${userId}`)
        );

        var flow = new DAuthFlow(
          generateOrkUrls(userNodes.map((un) => un.ork)),
          username
        );
        var keyTag = await flow.logIn(password);
        return resolve({ key: keyTag });
      } catch (error) {
        return reject(error);
      }
    });
  }

  logout() {
    this.key = null;
  }

  encrypt(msg) {
    if (this.key == null) throw "You must be logged in to encrypt";
    return this.key.encryptStr(msg);
  }

  decrypt(cipher) {
    if (this.key == null) throw "You must be logged in to decrypt";
    return this.key.decryptStr(cipher);
  }

  async recover(username) {
    var userId = encodeBase64Url(new IdGenerator(username).buffer);
    var userNodes = JSON.parse(
      await get(`${this.serverUrl}/GetUserNodes/${userId}`)
    );

    var flow = new DAuthFlow(
      generateOrkUrls(userNodes.map((un) => un.ork)),
      username
    );
    flow.Recover(username);
  }

  reconstruct(username, shares, newPass) {
    return new Promise(async (resolve, reject) => {
      try {
        var userId = encodeBase64Url(new IdGenerator(username).buffer);
        var userNodes = JSON.parse(
          await get(`${this.serverUrl}/GetUserNodes/${userId}`)
        );
        var urls = generateOrkUrls(userNodes.map((un) => un.ork));
        var flow = new DAuthFlow(urls, username);

        return resolve(await flow.Reconstruct(shares, newPass, urls.length));
      } catch (error) {
        return reject(error);
      }
    });
  }
}

function post(url, data) {
  return new Promise(async (resolve, reject) => {
    var r = (await request.post(url).send(data)).body;

    return r.success ? resolve(r.content) : reject(r.error);
  });
}

function get(url) {
  return new Promise(async (resolve, reject) => {
    var r = (await request.get(url)).body;
    return r.success ? resolve(r.content) : reject(r.error);
  });
}

function generateOrkUrls(ids) {
  return ids.map((id) => `https://${id}.azurewebsites.net`);
}

export default Tide;
