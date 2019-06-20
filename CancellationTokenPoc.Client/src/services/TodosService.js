import axios from 'axios';

export default new class TodosService {

    async getTodos(searchTerms, cancelTokenSource) {

        let requestConfig = {
            params: {
              searchTerms: searchTerms
            }
          };

        if(cancelTokenSource){
            requestConfig.cancelToken = cancelTokenSource.token;
        }


        try {
            const response = await axios.get('https://localhost:5001/api/todos', requestConfig);

            if(response.status === 200){
                return response.data;
            }else{
                //handle response code
            }

        } catch (error) {
            throw error;
        }
    }


}