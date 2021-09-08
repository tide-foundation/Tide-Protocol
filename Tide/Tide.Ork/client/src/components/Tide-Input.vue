<template>
  <div class="tide-input f-c">
    <input :type="type" :id="id" :value="modelValue" @input="updateModelValue" />
    <label :for="id"><slot></slot></label>
  </div>
</template>

<script setup lang="ts">
import { defineEmits, defineProps } from "vue";

const props = defineProps({
  id: {
    type: String,
    required: true,
  },
  type: {
    type: String,
    default: "text",
    required: false,
  },

  modelValue: {
    type: String,
    default: "",
    required: true,
  },
});

const emit = defineEmits(["update:modelValue"]);
const updateModelValue = (e: Event) => emit("update:modelValue", (e.target as HTMLInputElement).value);
</script>

<style lang="scss" scoped>
.tide-input {
  align-items: flex-start;
  position: relative;
  margin-bottom: 20px;
  input {
    width: 100%;
    border: 1px solid #eaebf5;
    background-color: white;

    &:hover {
      border-color: $primary;
    }

    &:focus {
      border-color: $primary-dark;
    }
  }

  label {
    pointer-events: none;
    transition: 0.2s all ease-in-out;
    position: absolute;
    color: #6e748a;
    font-size: 0.8rem;
    background-color: $background;
    line-height: 0.7rem;
    padding: 0 1px;
    transform: translate(12px, -24px);
  }

  input:hover + label {
    color: $primary;
  }

  input:focus + label {
    color: $primary-dark;
  }
}
</style>
