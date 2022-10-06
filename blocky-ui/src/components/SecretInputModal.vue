<template>
  <div v-if="isOpen">
    <form @submit.prevent="onSubmit">
      <modal @close="onModalClose">
        <template v-slot:header>
          <span class="text-xl cursor-default">Unblock {{ host }}</span>
        </template>
        <template v-slot:body>
          <div class="px-3 py-2">
            <input
                type="text"
                class="border border-purple-400 rounded h-8 py-1 px-3 w-full focus:outline-none focus:ring-2 focus:ring-purple-400 focus:shadow-lg"
                v-model="secret"
                :disabled="isSettingSecret"
                :placeholder="'Enter password to unblock ' + host"/>
          </div>
        </template>
        <template v-slot:footer>
          <div>
            <button 
              class="disabled:text-gray-400 disabled:border-gray-400 disabled:cursor-not-allowed hover:text-white hover:bg-purple-700 rounded font-bold px-3 py-1 mr-4 ease-linear transition-all duration-150"
              type="submit">
              Unblock
            </button>
            <button>Cancel</button>
          </div>
        </template>
      </modal>
    </form>
  </div>
</template>

<script lang="ts">
import {defineComponent, ref} from "vue";
import Modal from "./Modal.vue";

export default defineComponent({
  name: "SecretInputModal",
  components: {Modal},
  props: {
    isOpen: {type: Boolean, default: false},
    host: {type: String, required: true}
  },
  setup(props, context) {
    const secret = ref("")
    const isBusy = ref(false)

    const onSubmit = () => {

    }

    const onModalClose = () => {
      if (isBusy.value) {
        return
      }
      context.emit('secretInputModalCancelled')
    }

    return {
      onModalClose
    }
  }
})
</script>

<style scoped>

</style>