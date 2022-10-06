<template>
  <div
      class="fixed z-9998 top-0 left-0 w-full h-full bg-gray-900 bg-opacity-50 table transition-opacity duration-300 ease-in-out">
    <div class="align-middle table-cell" @click.native.self="$emit('close')">
      <div ref="dialog" class="rounded bg-white shadow-lg m-auto md:w-1/3 sm:w-1/2">
        <div class="transition-all duration-300 ">
          <div class="bg-gradient-to-r from-purple-400 to-purple-100 rounded-t p-1.5 cursor-move select-none"
               @mousedown="onMouseDown">
            <slot name="header">
              default header
            </slot>
          </div>
          <div>
            <slot name="body">
              default body
            </slot>
          </div>
          <div class="bg-purple-100 rounded-b p-1.5">
            <slot name="footer">
              default footer
              <button type="button" class="border" @click="$emit('close')">Close</button>
            </slot>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import {defineComponent, ref} from "vue";

export default defineComponent({
  name: "Modal",
  setup() {
    const mouseX = ref(0)
    const mouseY = ref(0)
    const dialog = ref<HTMLDivElement>()

    const onMouseMove = (e: MouseEvent) => {
      const dx = e.clientX - mouseX.value
      const dy = e.clientY - mouseY.value

      if (dialog.value) {
        const {left, top} = dialog.value.getBoundingClientRect()
        const newLeft = Math.max(0, left + dx)
        const newTop = Math.max(0, top + dy)
        dialog.value.style.left = `${newLeft}px`
        dialog.value.style.top = `${newTop}px`
        dialog.value.style.position = "fixed"
      }

      mouseX.value = e.clientX
      mouseY.value = e.clientY
    }

    const onMouseUp = () => {
      document.removeEventListener("mousemove", onMouseMove)
      document.removeEventListener("mouseup", onMouseUp)
    }

    const onMouseDown = (e: MouseEvent) => {
      mouseX.value = e.clientX
      mouseY.value = e.clientY
      document.addEventListener("mousemove", onMouseMove)
      document.addEventListener("mouseup", onMouseUp)
    };

    return {onMouseDown, onMouseUp, onMouseMove, dialog}
  }
})
</script>

<style scoped>

</style>