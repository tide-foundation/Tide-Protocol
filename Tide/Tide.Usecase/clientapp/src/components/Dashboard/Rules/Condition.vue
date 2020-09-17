<template>
  <div class="condition" :class="[`g-l-${condition.level}`]">
    <div class="condition-content">
      {{condition.index}}
      {{condition.level}}
      <span class="group-indicator">
        <div v-if="condition.level > 0" class="level-1">&nbsp;</div>
        <div v-if="condition.level > 1" class="level-2">&nbsp;</div>
        <div v-if="condition.level > 2" class="level-3">&nbsp;</div>
        <div v-if="condition.level > 3" class="level-4">&nbsp;</div>
      </span>
      <!-- <div :style="{'width':`${condition.level *20}px`}">&nbsp;</div> -->
      <button @click="$parent.insertNewLine(condition.index)">+</button>
      <button :class="{'selected' : condition.selected}" @click="condition.selected = !condition.selected">*</button>
      <select class="union-dropdown" v-model="condition.union" :class="{'invisible':condition.index == 0}">
        <option value="And">And</option>
        <option value="Or">Or</option>
      </select>
      <select v-model="condition.field">
        <option v-for="(value, propertyName) in values" :key="propertyName" :value="propertyName">{{propertyName}}</option>
      </select>
      <select v-model="condition.operator">
        <option v-for="operator in operators" :key="operator" :value="operator">{{operator}}</option>
      </select>
      <select v-model="condition.value" :disabled="condition.field === null">
        <option v-for="(value,i) in values[condition.field]" :key="i" :value="value">{{value}}</option>
      </select>
      <button :disabled="$parent.sortedIndexList.length == 1" @click="$parent.removeLineAtIndex(condition.index)">x</button>
    </div>
  </div>
</template>

<script>
export default {
    props: ["condition"],
    data: function() {
        return {
            levelColors: ["#ffffff", "#fdffda", "#edf29d", "#d4db55"],
            operators: ["=", ">", "<", ">=", "<=", "Contains", "Does Not Contain"],
            values: {
                "Vendor Industry": ["Tech", "Hospitality", "Charity", "Health"],
                Ethnicity: ["Asian", "Indian", "Black", "Hispanic", "White"],
                Location: ["Africa", "Asia", "The Caribbean", "America", "Europe", "Oceania"],
                Sex: ["Male", "Female", "Other lol"],
                Age: ["0-10", "11-20", "21-30", "31-40", "41-50", "51-60", "61-70", "71-80", "81-90", "91-100", "100+"]
            }
        };
    },
    watch: {
        // Auto select the first option
        "condition.field": function(newVal, oldVal) {
            this.$nextTick(() => {
                this.condition.value = this.values[this.condition.field][0];
            });
        }
    }
};
</script>

<style lang="scss" scoped>
.condition {
    width: 100%;
    margin-top: -1px;
    padding: 3px;
    border: 1px solid rgb(223, 223, 223);
    display: flex;
    flex-direction: row;
    align-items: center;
    height: 38px;

    // &.g-l-0 {
    //     background: white;
    // }
    // &.g-l-1 {
    //     background: #fbf2ec;
    // }
    // &.g-l-2 {
    //     background: #ffe0e0;
    // }
    // &.g-l-3 {
    //     background: #ffd2d2;
    // }

    .condition-content {
        width: 100%;
        display: flex;
        flex-direction: row;
        align-items: center;
        justify-content: space-between;

        .group-indicator {
            position: relative;
            display: flex;
            flex-direction: row;
            div {
                position: absolute;

                height: 38px !important;
                margin-top: -19px;

                background: red;
                border: 1px solid black;
                border-right: 0px;

                &.level-1 {
                    width: 20px;
                    background: #fbf2ec;
                    transform: translate(-10px, 0);
                }

                &.level-2 {
                    width: 13.33px;
                    background: #ffe0e0;
                    transform: translate(-5px, 0);
                }

                &.level-3 {
                    width: 10px;
                    background: #ffc4c4;
                    transform: translate(0px, 0);
                }

                &.level-4 {
                    width: 3px;
                    background: #ffa3a3;
                    transform: translate(5px, 0);
                }
            }
        }

        input,
        select {
            border: 1px solid gray;
            height: 30px;
            padding: 3px;
            width: 200px;

            &.union-dropdown {
                width: 60px;
            }

            &.invisible {
                opacity: 0;
                pointer-events: none;
            }

            &:disabled {
                opacity: 0.3;
            }
        }

        input {
        }

        select {
        }

        button {
            width: 30px;

            &.selected {
                background-color: rgba(103, 228, 250, 0.76);
            }
        }
    }
}
</style>