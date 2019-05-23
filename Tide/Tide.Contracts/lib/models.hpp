#include <eosio/eosio.hpp>
#include <string>

using namespace eosio;
using std::string;

struct [[eosio::table]] vendor
{
    uint64_t id;
    name account;
    string public_key;
    string desc;

    uint64_t primary_key() const { return id; }
};
