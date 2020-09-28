<template>
  <div class="rule">
    <h3>{{rule.ruleId.toString()}} - {{rule.name}}</h3>
    <div id="info-bar">
      <div class="input-g">
        <label for>State</label>
        <button>State</button>
      </div>
      <div class="input-g">
        <label for>Destination</label>
        <button>Destination</button>
      </div>
      <div class="input-g">
        <label for>Expiration</label>
        <button>Expiration</button>
      </div>
      <div class="input-g">
        <label for>Tag</label>
        <button>Tag</button>
      </div>
      <div class="input-g">
        <label for>Action</label>
        <button>{{rule.action}}</button>
      </div>
      <div class="input-g">
        <label for>Automation</label>
        <button>Automation</button>
      </div>
    </div>
    <span>Conditions</span>

    <div id="conditions" v-if="!saving">
      <div v-if="!isOverridden">
        <button id="group-btn" v-if="!canUngroup" :disabled="rule.condition.filter(c=>c.selected).length < 2 || !canGroup" @click="groupSelected(1)">Group Selected {{errorMsg != null ? `(${errorMsg})` : '' }}</button>
        <button id="group-btn" v-if="canUngroup" @click="groupSelected(-1)">Ungroup Selected</button>
        <Condition v-for="condition in sortedIndexList" :key="condition.id" :condition="condition"></Condition>

        <div class="mt-2 space-between">
          <button @click="newCondition">NEW CONDITION</button>
          <button @click="saveRule">SAVE RULE</button>
        </div>
      </div>
      <div v-else id="overriden-panel">
        Overriden
        <button @click="removeOverride">Remove override</button>
      </div>
    </div>
    <div v-else>SAVING</div>
  </div>
</template>

<script>
import Condition from "./Condition.vue";
export default {
    props: ["rule"],
    components: { Condition },
    data: function() {
        return {
            errorMsg: "",
            saving: false
        };
    },
    computed: {
        isOverridden: function() {
            return typeof this.rule.condition == "boolean";
        },
        sortedIndexList: function() {
            return this.rule.condition.sort((a, b) => a.index - b.index);
        },
        selectedConditions: function() {
            return this.sortedIndexList.filter(c => c.selected);
        },
        displayConditions: function() {
            var groupedConditions = [{ level: this.sortedIndexList[0].level, conditions: [], shut: false }];
            var lastGroup = groupedConditions[groupedConditions.length - 1];
            this.sortedIndexList.forEach(condition => {
                if (condition.level == lastGroup.level) {
                    // If the same as last group
                    lastGroup.conditions.push(condition);
                } else if (condition.level > lastGroup.level) {
                    // If more than last group, create a new group
                    groupedConditions.push({ level: condition.level, conditions: [condition], shut: false });
                    lastGroup = groupedConditions[groupedConditions.length - 1];
                } else {
                    // If less, iterate backwards to find a valid group
                    groupedConditions.forEach(group => {
                        if (group.level > condition.level) group.shut = true;
                    });

                    let added = false;
                    for (let i = groupedConditions.length - 1; i >= 0; i--) {
                        const group = groupedConditions[i];
                        if (group.level == condition.level && !group.shut) {
                            group.conditions.push(condition);
                            added = true;
                            lastGroup = group;
                            break;
                        }
                    }
                    if (!added) {
                        groupedConditions.push({ level: condition.level, conditions: [condition], shut: false });
                        lastGroup = groupedConditions[groupedConditions.length - 1];
                    }
                }
            });

            // var nestedConditionGroups = [];
            // var previous;
            // var levelGroups = [];
            // groupedConditions.forEach(group => {
            //     if (nestedConditionGroups.length == 0) {
            //         nestedConditionGroups = [group];
            //         previous = nestedConditionGroups[0];
            //         levelGroups[previous.level] = previous;
            //     } else {
            //         if (previous.level == group.level - 1) {
            //             // if this new group is higher, add it to inner
            //             previous.inner = group;
            //             levelGroups[previous.level] = previous;
            //             previous = previous.inner;
            //             levelGroups[previous.level] = previous;
            //         } else {
            //             var applicableGroup = levelGroups[group.level];

            //             applicableGroup.inner = group;
            //             previous = applicableGroup.inner;
            //         }
            //     }
            // });

            // console.log(nestedConditionGroups);

            return groupedConditions;
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
        removeOverride() {
            this.rule.condition = [];
            this.newCondition();
        },
        newCondition() {
            var id = this.rule.condition.length == 0 ? 0 : Math.max(...this.rule.condition.map(o => o.index), 0) + 1;
            this.createLineAtIndex(id, 0);
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
        },
        insertNewLine(index) {
            var currentCondition = this.sortedIndexList[index];
            // Move current lines up index
            for (let i = 0; i < this.rule.condition.length; i++) {
                if (i >= index) {
                    this.rule.condition[i].index++;
                }
            }
            // insert new line at index
            this.createLineAtIndex(index, currentCondition.level);
        },
        createLineAtIndex(index, level) {
            this.rule.condition.push({
                id: this.$helper.generateUniqueId(),
                index: index,
                selected: false,
                union: "&&",
                field: "",
                operator: "==",
                value: "",
                level: level
            });
        },
        removeLineAtIndex(index) {
            // Remove the line
            this.rule.condition = this.rule.condition.filter(c => c.index != index);

            // Fix the remaining indexes
            for (let i = 0; i < this.sortedIndexList.length; i++) {
                this.sortedIndexList[i].index = i;
            }
        },
        async saveRule() {
            this.saving = true;
            var conditions = JSON.parse(JSON.stringify(this.rule.condition));

            function setUnion(condition) {
                if (condition.index == 0) delete condition.union;
                // else condition.union = condition.union == "And" ? "&&" : "||";
            }

            function setField(condition) {
                condition.field = `DateInfo.${condition.field}`;
            }

            conditions.forEach(condition => {
                if (condition.index == 0) delete condition.union;
                // setUnion(condition);
                // setField(condition);
                delete condition.index;
                delete condition.id;
                delete condition.selected;
            });

            conditions = JSON.stringify(conditions);

            this.$loading(true, "Creating Tide account...");
            try {
                var result = await this.$tide.updateCondition(this.rule.ruleId, conditions);
                this.$bus.$emit("show-message", "Rule updated successfully");
            } catch (thrownError) {
                console.log(thrownError);

                this.$bus.$emit("show-error", "Failed saving rule... :'(");
            } finally {
                this.$loading(false, "");
                this.$parent.deselectRule();
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
                position: relative;
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

        #overriden-panel {
            width: 100%;
            min-height: 100px;
        }
    }
}
</style>