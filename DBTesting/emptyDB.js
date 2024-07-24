const { MongoClient } = require('mongodb');
const {connectDB, getDB, closeClient} = require('db');

// Connect to MongoDB Atlas


async function dropAllCollections() {

  try {
    await connectDB();
    const db = getDB();

    // Get all collections
    console.log("getting collections...");
    const collections = await db.collections();
    // Drop each collection
    for (let collection of collections) {
      await collection.drop();
      console.log(`Dropped collection: ${collection.collectionName}`);
    }

    console.log('All collections dropped');
  } catch (err) {
    console.error('Error dropping collections:', err);
  } finally {
    await closeClient();
    console.log('Connection closed');
  }
}

dropAllCollections();
