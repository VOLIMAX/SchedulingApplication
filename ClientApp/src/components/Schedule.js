import React, { Component } from 'react';
//import { useForm } from "react-hook-form";

export class Schedule extends Component {
    static displayName = Schedule.name;

    constructor(props) {
        super(props);
        this.state = {
            items: [],
            isLoaded: false,
            error: null
        };
    }

    componentDidMount() {
        this.populateWeatherData();
    }

    static renderForecastsTable(items) {
        return (
            // TODO: user types 3 inputs and clicks "Find solution" button, 
            // I pass it to the backend, perform calculatins, get data back, and only then show this table
            // Знайти в реакті щось схоже на ngIf
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                    <tr>
                        <th>Date</th>
                        <th>Temp. (C)</th>
                        <th>Temp. (F)</th>
                        <th>Summary</th>
                    </tr>
                </thead>
                <tbody>
                    {items.map(item =>
                        <tr key={item.date}>
                            <td>{item.date}</td>
                            <td>{item.temperatureC}</td>
                            <td>{item.temperatureF}</td>
                            <td>{item.summary}</td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }

    render() {
        let contents = this.state.isLoaded
            ? Schedule.renderForecastsTable(this.state.items)
            : <p><em>Loading...</em></p>;

        //const { register, handleSubmit } = useForm();
        //const onSubmit = data => console.log(data);

        return (
            <div>
                <h1 id="tabelLabel">Schedule</h1>
                <p>This component creates an optimal shifts schedule for the security firm employees</p>
                {/* <form onSubmit={handleSubmit(onSubmit)}>          
                <input type="number" {...register("numberOfNurses", { required: true, min: 0, max: 99 })} />
                <input type="number" {...register("numberOfDays", { required: true, min: 0, max: 99 })} />
                <input type="number" {...register("numberOfShifts", { required: true, min: 0, max: 99 })} />
                <input type="submit" />
                </form> */}
                <div style={{ marginTop: "20px" }}>{contents}</div>
            </div>
        );
    }

    async populateWeatherData() {
        await fetch('WeatherForecast', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                numberOfDays:   1,//need number
                numberOfGuards: 1,//need number
                numberOfShifts: 1,//need number
            })
        })
        .then(res => res.json())
        .then(
            (data) => {
                this.setState({
                    isLoaded: true,
                    items: data
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
}
