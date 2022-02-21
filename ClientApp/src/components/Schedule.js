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
            statistics: [],
            listsWithEachSolution: {},
            isFormSubmitted: false,
            isLoaded: false,
            error: null
        };
    }

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
                        listsWithEachSolution: data.listsWithEachSolution
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

    formikForm() {
        const initialValues = {daysNum: '', guardsNum: '', shiftsNum: ''};

        return (
            <div className="App">
                <Formik
                    initialValues={initialValues}
                    onSubmit={async (values) => {
                        this.setState({
                            data: { daysNum: values.daysNum, guardsNum: values.guardsNum, shiftsNum: values.shiftsNum, isFormSubmitted: true }
                        }, 
                        this.populateSchedulingData)
                    }}
                >
                    <div className="section">
                        <Form>
                            <label>
                                Number of days
                                <Field name="daysNum"/>
                            </label>
                            <label>
                                Number of guards
                                <Field name="guardsNum"/>
                            </label>
                            <label>
                                Number of shifts
                                <Field name="shiftsNum"/>
                            </label>
                            <button type="submit">Submit</button>
                        </Form>
                    </div>
                </Formik>
            </div>
        );
    }

    renderSchedulingTable() {
        return (<div>Render table</div>
            // TODO: understand why this function isn't shown after submitting, show table, add reset state button, groom fields
            
        );
    }

    render() {
        let contents = this.state.isLoaded
            ? this.renderSchedulingTable()
            : <p><em>Loading...</em></p>;
        
        

        return (
            <div>
                <h1 id="tabelLabel">Schedule</h1>
                <p>This component creates an optimal shifts schedule for the security firm employees</p>
                {this.formikForm()}
                {this.state.isFormSubmitted ? <div style={{marginTop: "20px"}}>{contents}</div> : null}
            </div>
        );
    }
}
