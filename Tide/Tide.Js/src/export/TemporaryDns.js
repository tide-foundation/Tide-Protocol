import superagent from "superagent";

class TemporaryDns {
  async doesUserExist(uid) {
    return (await superagent.get(`https://ork-1.azurewebsites.net/api/TemporaryDns/UserExists/${uid}`)).body;
  }

  async setUserOrks(uid, orks) {
    return await post(`https://ork-1.azurewebsites.net/api/TemporaryDns/UserOrks/${uid}`, orks);
  }

  async getUserOrks(uid) {
    return await get(`https://ork-1.azurewebsites.net/api/TemporaryDns/UserOrks/${uid}`);
  }

  async getGlobalOrks(mode) {
    return await get(`https://ork-1.azurewebsites.net/api/TemporaryDns/GlobalOrks/${mode}`);
  }
}

export default new TemporaryDns();

async function get(endpoint) {
  try {
    var resp = (await superagent.get(endpoint)).text;
    return JSON.parse(resp);
  } catch (error) {
    console.log(error.response.text);
    return null;
  }
}

async function post(endpoint, payload) {
  try {
    await superagent.post(endpoint, payload);
    return true;
  } catch (error) {
    console.log(error.response.text);
    return false;
  }
}
