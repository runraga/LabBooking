const {MongoClient} = require('mongodb');
require("dotenv").config();

const uri = process.env.MONGO_DB
const client = new MongoClient(uri, { useNewUrlParser: true, useUnifiedTopology: true });

let db;

async function connectDB() {
    try {
        await client.connect();
        db = client.db('eclipse-booking-system');
        console.log('Connected to MongoDB');
    } catch (error) {
        console.error(error);
    }
}

function getDB() {
    return db;
}
async function closeClient(){
    await client.close();
 }

module.exports = { connectDB, getDB, closeClient };



