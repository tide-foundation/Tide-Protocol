<template>
  <section id="tide-container">
    <div class="box" id="transactions">
      <table>
        <tr>
          <th>Balance</th>
          <th>Action</th>
          <th>Amount</th>
        </tr>

        <tr v-for="(trans, index) in transactions" :key="index">
          <td>{{ trans.balance }}</td>
          <td :class="trans.method">{{ trans.method }}</td>
          <td>{{ trans.tide }}</td>
        </tr>
      </table>

      <h5
        v-if="
          $store.getters.dealHistory == null ||
            $store.getters.dealHistory.length <= 0
        "
        class="loading-text"
      >
        Loading transactions...
      </h5>
      <Button
        v-if="$store.getters.dealHistory.length > 20"
        id="show-btn"
        @click="hideRemaining = !hideRemaining"
        >{{ hideRemaining ? "Show" : "Hide" }} remaining</Button
      >
    </div>
    <div class="box" id="spend">
      <div
        class="spend-box"
        v-for="(method, index) in spendMethods"
        :key="index"
      >
        <i
          :class="method.icon"
          :style="'color:' + method.color"
          class="fa fa-3x spend-icon"
        ></i>
        <div class="spend-desc">{{ method.desc }}</div>
        <button class="spend-btn">Action</button>
      </div>
    </div>
  </section>
</template>

<script>
export default {
  data() {
    return {
      hideRemaining: true,
      spendMethods: [
        {
          icon: "fa-home",
          color: "#01acfe",
          desc: "Put Tide towards your next rental bill"
        },
        {
          icon: "fa-usd",
          color: "#8FBE00",
          desc: "Withdraw to your bank account"
        },
        {
          icon: "fa-btc",
          color: "#F7931A",
          desc: "Exchange for Bitcoin"
        },
        {
          icon: "fa-exchange",
          color: "#CC2A41",
          desc: "Transfer to another account"
        }
      ]
    };
  },
  created() {},
  computed: {
    transactions() {
      var currentBalance = 0;
      var transactionList = [];
      this.$store.getters.dealHistory.forEach(function(element) {
        currentBalance += element.value;
        transactionList.push({
          method: "Credit",
          tide: element.value.toFixed(2),
          balance: currentBalance.toFixed(2)
        });
      });

      return transactionList
        .reverse()
        .slice(0, this.hideRemaining ? 20 : 20000);
    }
  }
};
</script>

<style scoped lang="scss">
#tide-container {
  display: flex;
  flex-direction: row;
}

#transactions {
  width: 30%;
  border-top: 2px solid #8fbe00;
}

#spend {
  width: 70%;
  border-top: 2px solid orange;
}

.Credit {
  color: #8fbe00;
}

.Debit {
  color: #cc2a41;
}

td,
th {
  border: 0px;
}

th {
  border-bottom: #d3dce2 1px solid;
}

.spend-box {
  padding: 20px;
  height: 70px;
  border-bottom: #d3dce2 1px solid;
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: space-around;
  i {
    text-align: center;
    width: 10%;
    color: orange;
  }

  .spend-desc {
    padding-left: 20px;
    font-size: 17px;
    width: 80%;
    user-select: none;
  }

  button {
    width: 10%;
    margin: 0px;
    height: 40px;
    background-color: #01acfe;
    border: 0px solid transparent;
    color: white;
  }

  button:hover {
    background-color: #556270;
  }
}

#show-btn {
  width: 100%;
  border: 0px;
  background-color: #8fbe00;
  color: white;
}

.list-item {
  display: inline-block;
  margin-right: 10px;
}
.list-enter-active,
.list-leave-active {
  transition: all 1s;
}
.list-enter, .list-leave-to /* .list-leave-active below version 2.1.8 */ {
  opacity: 0;
  transform: translateY(30px);
}

.loading-text {
  text-align: center;
}
</style>
