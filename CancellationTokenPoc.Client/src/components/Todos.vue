<template>
  <v-container>
    <v-layout row wrap>
        <v-flex>
            <h1>Todos</h1>
            <v-text-field label="Search" placeholder="Search" solo @input="getTodos"></v-text-field>
            <v-text-field label="Search debounce" placeholder="Search debounce" solo @input="getTodosDebounced"></v-text-field>
            <v-text-field label="Search cancel" placeholder="Search cancel" solo @input="getTodosCancellable"></v-text-field>
              <v-progress-linear v-if="loading" :indeterminate="true"></v-progress-linear>
            <v-list v-else two-line>
                <template v-for="(todo, index) in todos">
              <v-divider :key="index + '_devider'"></v-divider>
              <v-list-tile :key="index + '_list-title'">
                  <v-list-tile-content>
                      <v-list-tile-title v-html="todo.title"></v-list-tile-title>
                  </v-list-tile-content>
              </v-list-tile>
          </template>
        </v-list>
        </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import TodosService from '../services/TodosService';
import { debounce } from 'lodash';
import axios from 'axios';

  export default {
    data: () => ({
        loading: false,
        todos: [],
        getTodosCancelTokenSource: null,
    }),
    async mounted(){
       await this.getTodos();
    },
    methods:{
        async getTodos(newValue, cancelTokenSource){
            this.loading = true;

            console.log('Search terms: ' + newValue);

            try {
                this.todos = await TodosService.getTodos(newValue, cancelTokenSource);
            } catch(error){
                if (axios.isCancel(error)) {
                    console.log('Request canceled', error.message);
                } else {
                    // handle error
                }

            } finally{
                this.loading = false;
            }
         
        },
        getTodosDebounced: debounce(async function(newValue){
            this.getTodos(newValue);
        }, 500),
        getTodosCancellable: debounce(async function(newValue) {
            
            if(this.getTodosCancelTokenSource){
                this.getTodosCancelTokenSource.cancel();
            }
        
            this.getTodosCancelTokenSource = axios.CancelToken.source();
            await this.getTodos(newValue, this.getTodosCancelTokenSource);
        }, 500)
    }
  }
</script>
