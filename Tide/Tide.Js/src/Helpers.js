 /** @param {string|Uint8Array} input */
export function encodeBase64Url(input) {
  const text =
    typeof input === "string" ? input : Buffer.from(input).toString("base64");
  return text.replace(/\=/g, "").replace(/\+/g, "-").replace(/\//g, "_");
}

/** @param {bigInt.BigInteger} number */
export function encodeFromBig(number) {
  return encodeBase64Url(Buffer.from(number.toArray(256).value));
}