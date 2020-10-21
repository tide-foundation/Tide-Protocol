export default class AuthResult {
  constructor(result: boolean, error: string, token: string) {
    this.result = result;
    this.error = error;
    this.token = token;
  }

  result: boolean;
  error: string;
  token: string;
}
