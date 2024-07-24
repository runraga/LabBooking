const { MongoClient, ObjectId } = require('mongodb');
require('dotenv').config();
const {connectDB, getDB, closeClient} = require('db');

// Connect to MongoDB Atlas


async function run() {
    try {
        await connectDB();
        const database = getDB();

        const usersCollection = database.collection('users');
        const resourcesCollection = database.collection('resources');
        const ratesCollection = database.collection('rates');
        const bookingsCollection = database.collection('bookings');
        const requestsCollection = database.collection('requests');
        const userGroupsCollection = database.collection('user_groups');
        const projectsCollection = database.collection('projects');

        // Insert user_groups
        const user_groups = [
            { _id: new ObjectId(), name: "internal" },
            { _id: new ObjectId(), name: "external" },
        ];
        console.log("got to here");
        const userGroupResult = await userGroupsCollection.insertMany(user_groups);
        const userGroupIds = userGroupResult.insertedIds;

        if (!userGroupIds) {
            throw new Error('User groups insertion failed');
        }

        // Insert Users
        const users = [
            { _id: new ObjectId(), name: "Alice", email: "alice@example.com", user_groups: [userGroupIds[0], userGroupIds[1]] },
            { _id: new ObjectId(), name: "Bob", email: "bob@example.com", user_groups: [userGroupIds[0]] }
        ];
        const userResult = await usersCollection.insertMany(users);
        const userIds = userResult.insertedIds;

        if (!userIds) {
            throw new Error('Users insertion failed');
        }

        // Insert Resources
        const resources = [
            { _id: new ObjectId(), name: "Microscope", type: "Imaging" },
            { _id: new ObjectId(), name: "Eclipse", type: "Mass Spectrometer" }
        ];
        const resourceResult = await resourcesCollection.insertMany(resources);
        const resourceIds = resourceResult.insertedIds;

        if (!resourceIds) {
            throw new Error('Resources insertion failed');
        }

        // Insert Rates
        const rates = [
            { _id: new ObjectId(), resources: resourceIds[0], rate: 50, currency: "USD", user_groups: userGroupIds[0] },
            { _id: new ObjectId(), resources: resourceIds[0], rate: 100, currency: "USD", user_groups: userGroupIds[1] },
            { _id: new ObjectId(), resources: resourceIds[1], rate: 175, currency: "USD", user_groups: userGroupIds[0] },
            { _id: new ObjectId(), resources: resourceIds[1], rate: 150, currency: "USD", user_groups: userGroupIds[1] }
        ];
        const rateResult = await ratesCollection.insertMany(rates);
        const rateIds = rateResult.insertedIds;

        if (!rateIds) {
            throw new Error('Rates insertion failed');
        }

        // Insert Requests
        const requests = [
            { _id: new ObjectId(), users: userIds[0], name: "Proteomics Request", account_number: "19300001" },
            { _id: new ObjectId(), users: userIds[1], name: "HDX Request", account_number: "RG.IMCB.123456" },
        ];
        const requestResult = await requestsCollection.insertMany(requests);
        const requestIds = requestResult.insertedIds;

        if (!requestIds) {
            throw new Error('Requests insertion failed');
        }

        // Insert projects
        const projects = [
            { _id: new ObjectId(), name: "24001_an_eclipse_project", requests: [requestIds[0]], bookings: [] }
        ];
        const projectResult = await projectsCollection.insertMany(projects);
        const projectIds = projectResult.insertedIds;

        if (!projectIds) {
            throw new Error('Projects insertion failed');
        }

        // Insert Bookings
        const bookings = [
            {
                _id: new ObjectId(),
                resources: resourceIds[0],
                users: userIds[0],
                rates: rateIds[0],
                start: new Date("2024-07-17T10:00:00Z"),
                duration: 120,
                project: null
            },
            {
                _id: new ObjectId(),
                resources: resourceIds[1],
                users: userIds[1],
                rates: rateIds[1],
                start: new Date("2024-07-18T14:00:00Z"),
                duration: 90,
                project: null
            }
        ];
        await bookingsCollection.insertMany(bookings);

        console.log('Data inserted successfully');
    } catch (error) {
        console.error('An error occurred:', error);
    } finally {
        await closeClient();
        console.log("client closed");
    }
}

run().catch(console.dir);
