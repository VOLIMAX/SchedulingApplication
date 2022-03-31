import React, {Component} from 'react';
import {Formik, Field, Form, useField, useFormikContext} from 'formik';
import './Schedule.css';

export class Schedule extends Component {
    static displayName = Schedule.name;

    constructor(props) {
        super(props);
        this.state = {
            data: {
                daysNum: 0,
                guardsNum: 0,
                shiftsNum: 0,
            },
            statistics: {},
            solutionsInfoLists: [],
            solutions: [],
            isLoaded: false,
            error: null
        };
    }

    //TODO: Rework field validation someday

    // validate = value => {
    //     let errorMessage;
    //     if (value <= 0) {
    //         errorMessage = 'Insert number which has bigger value than zero';
    //         console.log(errorMessage);
    //     }
    //     return errorMessage;
    // };

    async populateSchedulingData() {
        await fetch(`api/Schedule?Days=${this.state.data.daysNum}&Guards=${this.state.data.guardsNum}&Shifts=${this.state.data.shiftsNum}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(res => res.json())
            .then(
                (data) => {
                    this.setState({
                        isLoaded: true,
                        statistics: data.statistics,
                        solutionsInfoLists: data.solutionsInfoLists,
                        solutions: data.solutions
                    });
                },
                (error) => {
                    this.setState({
                        isLoaded: true,
                        error
                    });
                }
            )
    }
    
    reset = () => {
        this.setState({
            data: {
                daysNum: 0,
                guardsNum: 0,
                shiftsNum: 0,
            },
            statistics: {},
            solutionsInfoLists: [],
            solutions: [],
            isLoaded: false,
            error: null
        })
    }

    formikForm() {
        const initialValues = {daysNum: '', guardsNum: '', shiftsNum: ''};

        return (
            <div className="App">
                <Formik
                    initialValues={initialValues}
                    onSubmit={async (values) => {
                        this.reset()
                        this.setState({
                                data: {daysNum: values.daysNum, guardsNum: values.guardsNum, shiftsNum: values.shiftsNum}
                            }, 
                            this.populateSchedulingData)
                    }}
                    onReset={async () => {
                        this.reset();
                    }}
                >
                    <div className="section">
                        <Form>
                            <label>
                                Кількість днів
                                <Field validate={this.validate} name="daysNum" placeholder="0"/>
                            </label>
                            <label>
                                Кількість охоронців
                                <Field validate={this.validate} name="guardsNum" placeholder="0"/>
                            </label>
                            <label>
                                Кількість змін
                                <Field validate={this.validate} name="shiftsNum" placeholder="0"/>
                            </label>
                            <div className="container-fluid justify-content-around row">
                                <button type="submit">Обчислити</button>
                                <button type="reset">Очистити</button>
                            </div>
                        </Form>
                    </div>
                </Formik>
            </div>
        );
    }

    renderSchedulingTable(solutionsInfoLists, solutions) {
        // TODO: 1) make solutions as table headers in a separate list
        //       2) add reset state button
        return (
            solutions === null || typeof solutions === 'undefined' || solutions.length === 0
                ? "Обчислення повернуло 0 результатів. Спробуйте ввести інші параметри" :
                <table className='table table-striped'>
                    <thead className="table-dark">
                    <tr className="d-flex flex-row justify-content-around">
                        {solutions.map(solution =>
                            <th>
                                <div>{solution}</div>
                            </th>
                        )}
                    </tr>
                    </thead>
                    <tbody>
                    <tr className="d-flex flex-row justify-content-around">
                        {solutionsInfoLists.map(solutionList =>
                            <td>
                                <div>
                                    {solutionList.map(item =>
                                        <div>{item}</div>)}
                                </div>
                            </td>
                        )}
                    </tr>
                    </tbody>
                </table>
        );
    }


    render() {
        let contents = this.state.isLoaded
            ? this.renderSchedulingTable(this.state.solutionsInfoLists, this.state.solutions)
            : <p><em>Заповніть поля та нажміть на кнопку "Обчислити", щоб почати обчислення</em></p>;

        return (
            <div>
                <h1 id="tableLabel">Графік Чергувань</h1>
                <p>Цей модуль створює оптимальний графік змін для працівників охоронної фірми</p>
                {this.formikForm()}
                <div>{contents}</div>
            </div>
        );
    }
}
