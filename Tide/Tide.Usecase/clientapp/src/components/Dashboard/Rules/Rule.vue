<template>
  <div class="rule">
    <h3>{{rule.id}} - {{rule.name}}</h3>
    <div id="info-bar">
      <div class="input-g">
        <label for>State</label>
        <button>{{rule.state}}</button>
      </div>
      <div class="input-g">
        <label for>Destination</label>
        <button>{{rule.destination}}</button>
      </div>
      <div class="input-g">
        <label for>Expiration</label>
        <button>{{ rule.expiration | moment("D / MM / YYYY")}}</button>
      </div>
      <div class="input-g">
        <label for>Tag</label>
        <button>{{rule.tag}}</button>
      </div>
      <div class="input-g">
        <label for>Action</label>
        <button>{{rule.action}}</button>
      </div>
      <div class="input-g">
        <label for>Automation</label>
        <button>{{rule.automation}}</button>
      </div>
    </div>
    <span>Conditions</span>
    <div id="conditions">
      <button id="group-btn" v-if="!canUngroup" :disabled="rule.conditions.filter(c=>c.selected).length < 2 || !canGroup" @click="groupSelected(1)">Group Selected {{errorMsg != null ? `(${errorMsg})` : '' }}</button>
      <button id="group-btn" v-if="canUngroup" @click="groupSelected(-1)">Ungroup Selected</button>
      <Condition v-for="condition in sortedIndexList" :key="condition.id" :condition="condition"></Condition>

      <div class="mt-2 space-between">
        <button @click="newCondition">NEW CONDITION</button>
        <button @click="saveRule">SAVE RULE</button>
      </div>
    </div>
  </div>
</template>

<script>
import Condition from "./Condition.vue";
export default {
    props: ["rule"],
    components: { Condition },
    data: function() {
        return { errorMsg: "" };
    },
    computed: {
        sortedIndexList: function() {
            return this.rule.conditions.sort((a, b) => a.index - b.index);
        },
        selectedConditions: function() {
            return this.sortedIndexList.filter(c => c.selected);
        },
        canGroup: function() {
            var base = this;
            this.errorMsg = null;
            function error(err) {
                base.errorMsg = err;
                return false;
            }

            // Minimum count
            if (this.selectedConditions.length < 2) return error("Minimum of 2 conditions required");

            // Ensure no intersection (start/end different levels)
            var ends = this.getGroupEnds();
            if (ends.start.level != ends.end.level) return error("Cannot interest groups");

            // Ensure no jumping between two groups with a gap
            for (let i = ends.start.index; i < ends.end.index + 1; i++) {
                const condition = this.sortedIndexList[i];
                if (condition.level != ends.start.level) return error("Group ends must reside within a single group");
            }

            // Ensure no gaps in selected
            const uniqueIndexes = [...new Set(this.selectedConditions.map(c => c.index))];
            for (let i = 0; i < uniqueIndexes.length; i++) {
                if (i == uniqueIndexes.length - 1) return true;
                if (uniqueIndexes[i] + 1 != uniqueIndexes[i + 1]) return error("Groups cannot have gaps");
            }
        },
        canUngroup: function() {
            if (!this.canGroup) return false; // Check if can group for the gaps and intersections.

            if (this.selectedConditions[0].level == 0) return false; // If we're on level 0, we can't ungroup

            // Ensure the whole level group is selected
            var ends = this.getGroupEnds();
            if (ends.start.index != 0 && this.sortedIndexList[ends.start.index - 1].level == ends.start.level) return false;
            if (ends.end.index != this.sortedIndexList[this.sortedIndexList.length - 1].index && this.sortedIndexList[ends.end.index + 1].level == ends.end.level) return false;

            return true;
        }
    },
    methods: {
        newCondition() {
            this.createLineAtIndex(Math.max(...this.rule.conditions.map(o => o.index), 0) + 1, 0);
        },
        error(msg) {
            this.error = msg;
            return false;
        },
        getGroupEnds() {
            return { start: this.selectedConditions[0], end: this.selectedConditions[this.selectedConditions.length - 1] };
        },
        groupSelected(by) {
            // Find the start and end conditions
            var ends = this.getGroupEnds();

            // Add a level to everything in between
            var innerConditions = this.sortedIndexList.filter(c => c.index >= ends.start.index && c.index <= ends.end.index).forEach(c => (c.level += by));
            this.sortedIndexList.filter(c => c.selected).forEach(c => (c.selected = false));
            ends.start.isStart = by == 1;
            ends.start.startBorderLevel = ends.start.level;
            ends.end.isEnd = by == 1;
            ends.end.endBorderLevel = ends.end.level;
            console.log(ends.end.isEnd);
        },
        insertNewLine(index) {
            var currentCondition = this.sortedIndexList[index];
            // Move current lines up index
            for (let i = 0; i < this.rule.conditions.length; i++) {
                if (i >= index) {
                    this.rule.conditions[i].index++;
                }
            }
            // insert new line at index
            this.createLineAtIndex(index, currentCondition.level);
        },
        createLineAtIndex(index, level) {
            this.rule.conditions.push({
                id: this.$helper.generateUniqueId(),
                index: index,
                selected: false,
                union: "",
                field: "",
                operator: "",
                value: "",
                level: level
            });
        },
        removeLineAtIndex(index) {
            // Remove the line
            this.rule.conditions = this.rule.conditions.filter(c => c.index != index);

            // Fix the remaining indexes
            for (let i = 0; i < this.sortedIndexList.length; i++) {
                this.sortedIndexList[i].index = i;
            }
        },
        async saveRule() {
            this.$loading(true, "Creating Tide account...");
            try {
                var result = await this.$tide.allowTags([this.rule]);
                this.$bus.$emit("show-message", "Rule updated successfully");
            } catch (thrownError) {
                console.log(thrownError);
                this.$bus.$emit("show-error", "Failed saving rule... :'(");
            } finally {
                this.$loading(false, "");
            }
        }
    }
};
</script>

<style lang="scss" scoped>
.rule {
    width: 100%;

    display: flex;
    flex-direction: column;

    h3 {
        display: block;
    }
    #info-bar {
        display: flex;
        flex-direction: row;
        justify-content: space-evenly;
        padding: 0 10px 20px 10px;
        flex-wrap: wrap;
        background-color: rgb(243, 243, 243);

        .input-g {
            margin-top: 20px;
            width: 33%;
            label {
                display: block;
            }
            button {
                width: 120px;
            }
        }
    }

    span {
        margin-top: 10px;
    }

    #conditions {
        padding: 10px;
        display: flex;
        flex-direction: column;

        #group-btn {
            margin-bottom: 10px;
        }

        .level {
        }
    }
}
</style>