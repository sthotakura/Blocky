<template>
  <div class="flex flex-col items-center">
    <div class="spinner" :class="'w-' + size + ' h-' + size" ref="spinner"></div>
    <div v-if="showText" class="mt-3 text-2xl">
      {{ text }}
    </div>
  </div>
</template>

<script lang="ts">
import {defineComponent, onMounted, ref, watch} from "vue";

export default defineComponent({
  name: "Spinner",
  props: {
    text: {type: String, default: "Loading . . ."},
    showText: {type: Boolean, default: true},
    size: {type: Number, default: 20}
  },
  setup(props) {
    const spinner = ref<HTMLDivElement>()

    const onSizeChanged = () => {
      const spinnerDiv = spinner.value
      if (spinnerDiv) {
        spinnerDiv.style.border = props.size * 2 / 3 + "px solid #f3f3f3"
        spinnerDiv.style.borderTop = props.size * 2 / 3 + "px solid rgb(91, 33, 182)"
      }
    }

    watch(() => props.size, (_, __) => onSizeChanged(), {immediate: true})

    onMounted(() => onSizeChanged())

    return {
      spinner, onSizeChanged
    }
  },
})
</script>

<style scoped>
.spinner {
  border-radius: 50%;
  -webkit-animation: spin 2s linear infinite; /* Safari */
  animation: spin 2s linear infinite;
}

/* Safari */
@-webkit-keyframes spin {
  0% {
    -webkit-transform: rotate(0deg);
  }
  100% {
    -webkit-transform: rotate(360deg);
  }
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}
</style>