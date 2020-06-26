import DAuthFlow from "./dauth/DAuthFlow";
import IdGenerator from "./IdGenerator";
import request from "superagent";
import { encodeBase64Url } from "./Helpers";

class Tide {
  constructor(vendorId, serverUrl, orkNodes) {
    this.vendorId = vendorId;
    this.serverUrl = serverUrl;
    this.orkNodes = orkNodes || [];
  }
  register(username, password, email) {
    return new Promise(async (resolve, reject) => {
      try {
        // Some local validation, which is all we can really do.
        if (username.length < 3 || password.length < 6)
          return reject("Invalid credentials");
          
        var flow = new DAuthFlow(this.orkNodes, username);
        var userId = encodeBase64Url(new IdGenerator(username).buffer);
        // Ask the vendor to create the user as a liability.
        await request
          .post(`${this.serverUrl}/CreateUser/${userId}`)
          .send(["ork-0", "ork-1", "ork-2"]);
        // Send all shards to selected orks
        var key = await flow.signUp(password, email, 2);
        // Finally, ask the vendor to confirm the user
        await request.get(`${this.serverUrl}/ConfirmUser/${userId}/`);

        this.key = key;
        resolve({ key: key });
      } catch (error) {
        reject(error);
      }
    });
  }
  login(username, password) {
    return new Promise(async (resolve, reject) => {
      try {
        var flow = new DAuthFlow(this.orkNodes, username);
        var keyTag = await flow.logIn(password);
        return resolve({ key: keyTag });
      } catch (error) {
        return reject(error);
      }
    });
  }

  logout(){
    this.key = null;
  }

  encrypt(msg){
    if(this.key == null) throw "You must be logged in to encrypt";
    return this.key.encryptStr(msg);
  }

  decrypt(cipher){
    if(this.key == null) throw "You must be logged in to decrypt";
    return this.key.decryptStr(cipher);
  }

  forgotPassword(username) {
    // Trigger selected orks to send email fragments
  }
  combineForgetPasswordFragments(fragments) {}
  resetPassword(username, password) {}
}

export default Tide;
