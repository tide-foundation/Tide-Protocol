export function getHashParams() {
  var hash = window.location.hash.substr(1);
  return hash.split("&").reduce(function(res, item) {
    var parts = item.split("=");
    res[parts[0]] = parts[1];
    return res;
  }, {});
}
