import { Store } from "./main";

// @ts-ignore
import Tide, { C25519Key } from "../../../../Tide.Js/src/export/TideAuthentication";

import { SESSION_ACCOUNT_KEY } from "@/assets/ts/Constants";

interface MainState extends Object {
  tide?: any;
  config: Config;
  action: AuthAction;
  account?: Account;
}

class MainStore extends Store<MainState> {
  protected data(): MainState {
    return {
      config: {} as Config,
      tide: undefined,
      action: "Login",
      account: undefined,
    };
  }

  initialize(config: Config) {
    this.state.config = config;
    this.state.tide = new Tide("VendorId", config.serverUrl, config.orks, config.vendorPublic);
  }

  async login(user: UserPass) {
    this.state.action = "Login";

    this.setAccount(await this.state.tide.loginJwt(user.username, user.password, await getServerTime()), "Login");

    if (this.state.account == null) throw new Error("Invalid account");
    return this.state.account;
  }

  async registerAccount(user: UserPass) {
    this.state.action = "Register";
    const serverTime = await getServerTime();

    this.setAccount(
      await this.state.tide.registerJwt(user.username, user.password, user.emails, this.state.config.orks, serverTime, this.state.config.orks.length),
      "Register"
    );

    return this.state.account;
  }

  setAccount(account: Account, action: AuthAction) {
    // Stringify the C25519Key
    account.encryptionKey = account.encryptionKey.toString();
    account.vuid = account.vuid.toString();
    this.state.account = account;
    this.state.action = action;

    localStorage.setItem(SESSION_ACCOUNT_KEY, JSON.stringify(account));
  }

  logout() {
    this.state.account = undefined;
    localStorage.removeItem(SESSION_ACCOUNT_KEY);
    localStorage.removeItem("silent_login_token"); // From tide-js
  }

  authenticationComplete() {
    const authResponse: AuthResponse = {
      publicKey: this.state.account!.cvkPublic,
      tideToken: this.state.account!.tideToken,
      action: this.state.action,
      vuid: this.state.account!.vuid,
    };
    //console.log(constructReturnUrl(this.state.config.returnUrl!, authResponse));
    window.location.replace(constructReturnUrl(this.state.config.returnUrl!, authResponse));
  }

  returnFormData(fields: any[]) {
    var returnData = fields.map((f) => {
      return {
        key: f.field,
        value: f.value,
      } as ReturnData;
    });

    window.location.replace(constructReturnUrl(this.state.config.returnUrl!, returnData));
  }

  async sendRecoveryEmails(username: string) {
    await this.state.tide.recover(username, this.state.config.orks);
  }

  async reconstructAccount(username: string, shares: string, newPassword: string) {
    await this.state.tide.reconstruct(username, shares, newPassword, this.state.config.orks);
  }

  async changePassword(newPassword: NewPassword) {
    await this.state.tide.changePassword(newPassword.password, newPassword.confirm, this.state.config.orks);
  }
}

const mainStore: MainStore = new MainStore();

export default mainStore;

async function getServerTime(): Promise<number> {
  return await (await fetch(`${mainStore.getState.config?.serverUrl}/tide-utility/servertime`)).json();
}

function constructReturnUrl(returnUrl: string, data: object) {
  if (!returnUrl.includes("reset=")) {
    return `${returnUrl}${returnUrl.includes("?") ? "&" : "?"}data=${encodeURIComponent(JSON.stringify(data))}`;
  }

  // Edge case for reset
  var redirect = returnUrl;

  var indexOfQueries = returnUrl.indexOf("?");

  if (indexOfQueries != -1) {
    var start = returnUrl.substr(0, indexOfQueries + 1);
    var end = returnUrl.substr(indexOfQueries + 1);

    var splitQueries = end.split("&");
    var newQueries = "";

    for (let i = 0; i < splitQueries.length; i++) {
      const query = splitQueries[i];

      var s = query.split("=");
      newQueries += `${s[0]}=${encodeURIComponent(s[1])}`;

      if (i <= splitQueries.length - 1) newQueries += "&";
    }

    redirect = start + newQueries;
  }

  return `${redirect}${redirect.includes("?") ? "&" : "?"}data=${encodeURIComponent(JSON.stringify(data))}`;
}
